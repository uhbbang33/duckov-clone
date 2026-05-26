using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private float _reloadDelay = 0.1f;
    [SerializeField] private float _soundLevelDuration = 0.1f;
    [SerializeField] private float _shootIgnoreRadius = 2f;
    [SerializeField] private float _spreadToAngle;
    [SerializeField] private AudioSource _playerShootingAudioSource;
    [SerializeField] private CameraShakeController _cameraShakeController;

    private GameObject _currentGunObject;
    private Gun _currentGun;
    private GunItem _currentGunItem;
    private InputActions _actions;
    private Player _player;
    private PlayerMove _playerMove;
    private PlayerEnemyScanner _playerEnemyScanner;
    private Inventory _inventory;
    private PlayerEquip _playerEquip;
    private PoolManager _poolManager;
    private GameManager _gameManager;
    private UIManager _uiManager;

    private PlayerFireState _state;
    private bool _isFirePressed;

    private Coroutine _fireCoroutine;
    private Coroutine _reloadCoroutine;
    private WaitForSeconds _waitforReloadDelay;
    private WaitForSeconds _waitforSoundLevelDuration;

    public event Func<float> OnFire;
    public event Action OnFireFail;

    public GameObject CurrentGunObject
    {
        get { return _currentGunObject; }
        set
        {
            _currentGunObject = value;

            if (_currentGunObject != null)
            {
                _currentGun = _currentGunObject.GetComponent<Gun>();
                _playerMove.LookBaseTransform = _currentGun.MuzzleTransform;
            }
            else
            {
                _currentGun = null;
                _playerMove.LookBaseTransform = null;
            }
        }
    }

    public GunItem CurrentGunItem
    {
        get { return _currentGunItem; }
        set { _currentGunItem = value; }
    }

    public PlayerFireState FireState
    {
        get { return _state; }
        set { _state = value; }
    }

    public bool IsFirePressed
    {
        get { return _isFirePressed; }
        set { _isFirePressed = value; }
    }

    private void Awake()
    {
        _state = PlayerFireState.Idle;
        _waitforReloadDelay = new WaitForSeconds(_reloadDelay);
        _waitforSoundLevelDuration = new WaitForSeconds(_soundLevelDuration);
    }

    private void Start()
    {
        _actions = GameManager.Instance.Actions;
        
        _actions.Player.Fire.performed += OnFirePerformed;
        _actions.Player.Fire.canceled += OnFireCanceled;
        _actions.Player.Reload.performed += OnReload;

        _player = GetComponent<Player>();
        _playerMove = GetComponent<PlayerMove>();
        _playerEquip = GetComponent<PlayerEquip>();
        _playerEnemyScanner = GetComponent<PlayerEnemyScanner>();

        _inventory = GameManager.Instance.Inventory;
        _poolManager = PoolManager.Instance;
        _gameManager = GameManager.Instance;
        _uiManager = UIManager.Instance;
    }

    private void OnDisable()
    {
        _isFirePressed = false;
        _actions.Player.Fire.performed -= OnFirePerformed;
        _actions.Player.Fire.canceled -= OnFireCanceled;
        _actions.Player.Reload.performed -= OnReload;
    }

    private void OnFirePerformed(InputAction.CallbackContext context)
    {
        _isFirePressed = true;

        if (_currentGunItem == null
            || _currentGunObject == null
            || _player.State == PlayerState.Running
            || _player.State == PlayerState.Rolling
            || _state != PlayerFireState.Idle
            || _uiManager.PeekStack() != null
            || Time.timeScale != 1f)
            return;

        if (_fireCoroutine != null)
            return;

        _fireCoroutine = StartCoroutine(FireRoutine());
    }

    private void OnFireCanceled(InputAction.CallbackContext context)
    {
        _isFirePressed = false;
    }

    private void OnReload(InputAction.CallbackContext context)
    {
        if (_currentGunItem == null
            || _currentGunObject == null)
            return;

        if (_state == PlayerFireState.Idle)
            Reload();
    }

    private void Fire()
    {

        float spread = OnFire?.Invoke() ?? 0f;

        Vector3 dir = GetFireDirection(spread);

        if (dir == Vector3.zero)
        {
            OnFireFail?.Invoke();
            return;
        }

        float speed = spread * _currentGunItem.SoundRange / 100f;
        _cameraShakeController.ShakeOnFire(-dir, speed);

        // bullet
        GameObject bulletObject = _poolManager.GetObject(PoolId.Bullet, _currentGun.MuzzleTransform, false);

        Bullet bullet = bulletObject.GetComponent<Bullet>();
        bullet.BulletDamage = _currentGunItem.Damage;
        bullet.Fire(dir, _currentGunItem.Range);


        // muzzle effect
        GameObject muzzleFlash = _poolManager.GetObject(PoolId.MuzzleFlash, _currentGun.MuzzleTransform, false);
        

        // Sound
        SoundManager.Instance.PlayGunSFX(_currentGunItem.ID, _playerShootingAudioSource);

        
        StartCoroutine(SoundLevelUpByFireRoutine());

        if (_gameManager.CurrentSceneName != SceneName.BunkerScene)
            _currentGunItem.CurrentAmmoCount -= 1;

        _playerEquip.RefreshHUDAmmoCountText();

    }

    private void Reload()
    {
        if (_currentGunItem.CurrentAmmoCount == _currentGunItem.MagazineCapacity)
            return;

        // 인벤토리에 탄환이 있는지 확인
        if (!_inventory.HasItem(_currentGunItem.BulletId))
            return;

        if (_fireCoroutine != null)
        {
            StopCoroutine(_fireCoroutine);
            _fireCoroutine = null;
        }
        
        if (_reloadCoroutine != null)
            StopCoroutine(_reloadCoroutine);
        _reloadCoroutine = StartCoroutine(ReloadRoutine());
    }

    #region Coroutine

    private IEnumerator FireRoutine()
    {
        while (_isFirePressed)
        {
            if (_currentGunItem.CurrentAmmoCount <= 0)
            {
                Reload();
                break;
            }
            else if (_state == PlayerFireState.Idle)
            {
                _state = PlayerFireState.Firing;

                if (_player.State != PlayerState.Running
                    && _player.State != PlayerState.Rolling)
                    Fire();

                yield return new WaitForSeconds(1.0f / _currentGunItem.Rps);

                _state = PlayerFireState.Idle;
            }
            else
                break;
        }

        _fireCoroutine = null;
    }

    private IEnumerator ReloadRoutine()
    {
        _state = PlayerFireState.Reloading;

        SoundManager.Instance.PlayReloadSFX(true, _playerShootingAudioSource);

        // TODO : 장전 시간 및 UI
        float currentReloadTime = 0f;
        float reloadTime = _currentGunItem.ReloadTime;
        while (currentReloadTime < reloadTime)
        {
            yield return _waitforReloadDelay;
            currentReloadTime += _reloadDelay;
        }

        if (_currentGunItem == null)
        {
            _state = PlayerFireState.Idle;
            yield break;
        }

        SoundManager.Instance.PlayReloadSFX(false, _playerShootingAudioSource);

        int maxReloadableAmmoCount = (int)_currentGunItem.MagazineCapacity - _currentGunItem.CurrentAmmoCount;

        // 인벤토리에서 가져올 수 있는 수량 체크 및 아이템 저장
        (int, AmmoItem) reloadable = _inventory.ReloadableAmmoCount(_currentGunItem.BulletId, maxReloadableAmmoCount);

        _currentGunItem.Ammo = reloadable.Item2;
        _currentGunItem.CurrentAmmoCount += reloadable.Item1;

        // ammo count Text
        _playerEquip.RefreshHUDAmmoCountText();
        _playerEquip.CurrentGunSlotRefreshUI();

        _state = PlayerFireState.Idle;

        if (_isFirePressed)
            _fireCoroutine = StartCoroutine(FireRoutine());

        yield return null;
    }

    private IEnumerator SoundLevelUpByFireRoutine()
    {
        float soundRange = _currentGunItem.SoundRange;

        _playerEnemyScanner.PlayerSoundLevel += soundRange;

        yield return _waitforSoundLevelDuration;

        _playerEnemyScanner.PlayerSoundLevel -= soundRange;
    }

    #endregion Coroutine

    #region Set Shoot Direction

    // 타겟의 월드 위치와 플레이어 사이의 거리 제곱값 return
    private float GetSqrDistanceToTarget(Vector3 targetPosition)
    {
        Vector3 distance = targetPosition - transform.position;
        return distance.sqrMagnitude;
    }

    // 마우스 에임이 가리키는 월드 좌표 return
    public Vector3 GetAimWorldPosition()
    {
        if (_currentGunItem == null)
            return Vector3.zero;

        //Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector2 mousePos = _playerMove.MousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            // 플레이어 위에 커서가 있을 경우
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer(Layer.Player))
                return Vector3.zero;
        }

        Plane groundPlane = new Plane(Vector3.up, _currentGun.MuzzleTransform.position);

        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 worldPos = ray.GetPoint(distance);

            // 무시 반경 내부일 경우
            if (GetSqrDistanceToTarget(worldPos) < _shootIgnoreRadius * _shootIgnoreRadius)
                return Vector3.zero;

            return worldPos;
        }

        return Vector3.zero;
    }

    private Vector3 GetFireDirection(float spread)
    {
        Vector3 aimTarget = GetAimWorldPosition();

        if (aimTarget == Vector3.zero)
            return Vector3.zero;

        Vector3 dir = (aimTarget - _currentGun.MuzzleTransform.position).normalized;

        if (spread <= 0f)
            return dir;

        float spreadAngle = spread * _spreadToAngle;
        float randomAngle = UnityEngine.Random.Range(-spreadAngle, spreadAngle);
        Vector3 spreadDir = Quaternion.AngleAxis(randomAngle, Vector3.up) * dir;

        return spreadDir.normalized;
    }

    #endregion Set Shoot Direction
}
