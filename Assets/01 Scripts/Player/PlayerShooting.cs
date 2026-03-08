using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private LayerMask _targetLayer;
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private float _reloadDelay = 0.1f;
    [SerializeField] private float _soundLevelDuration = 0.1f;

    private GameObject _currentGunObject;
    private Gun _currentGun;
    private GunItem _currentGunItem;
    private InputActions _actions;
    private PlayerMove _playerMove;
    private PlayerEnemyScanner _playerEnemyScanner;
    private Inventory _inventory;
    private PlayerEquip _playerEquip;
    private PoolManager _poolManager;

    private PlayerShootingState _state;
    private bool _isFirePressed;

    private Coroutine _fireCoroutine;
    private Coroutine _reloadCoroutine;
    private WaitForSeconds _waitforReloadDelay;
    private WaitForSeconds _waitforSoundLevelDuration;

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

    public PlayerShootingState State
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
        _state = PlayerShootingState.Idle;
        _waitforReloadDelay = new WaitForSeconds(_reloadDelay);
        _waitforSoundLevelDuration = new WaitForSeconds(_soundLevelDuration);
    }

    private void Start()
    {
        _actions = GetComponent<Player>().Actions;

        _actions.Player.Fire.performed += OnFire;
        _actions.Player.Fire.canceled += OnFire;
        _actions.Player.Reload.performed += OnReload;

        _playerMove = GetComponent<PlayerMove>();
        _inventory = GetComponent<Inventory>();
        _playerEquip = GetComponent<PlayerEquip>();
        _playerEnemyScanner = GetComponent<PlayerEnemyScanner>();

        _poolManager = PoolManager.Instance;
    }

    private void OnDisable()
    {
        _actions.Player.Fire.performed -= OnFire;
        _actions.Player.Fire.canceled -= OnFire;
        _actions.Player.Reload.performed -= OnReload;
    }

    private void OnFire(InputAction.CallbackContext context)
    {
        if (_currentGunItem == null
            || _currentGunObject == null
            || _playerMove.IsRun
            || _inventory.InventoryIsOpen)
        {
            _isFirePressed = false;
            return;
        }

        if (context.performed)
        {
            _isFirePressed = true;

            if (_state != PlayerShootingState.Idle)
                return;

            _fireCoroutine = StartCoroutine(FireRoutine());
        }
        else if (context.canceled)
        {
            _isFirePressed = false;

            if (_fireCoroutine != null)
                StopCoroutine(_fireCoroutine);

            if (_state == PlayerShootingState.Shooting)
                _state = PlayerShootingState.Idle;
        }
    }

    private void OnReload(InputAction.CallbackContext context)
    {
        if (_currentGunItem == null
            || _currentGunObject == null
            || _state != PlayerShootingState.Idle)
            return;

        Reload();
    }

    private void Fire()
    {
        _currentGunItem.CurrentAmmoCount -= 1;

        // bullet
        GameObject bullet = _poolManager.GetObject(PoolId.Bullet, _currentGun.MuzzleTransform, false);

        Vector3 dir = GetFireDirection();

        bullet.GetComponent<Bullet>().Fire(dir, _currentGunItem.Range);


        // muzzle effect
        GameObject muzzleFlash = _poolManager.GetObject(PoolId.MuzzleFlash, _currentGun.MuzzleTransform, false);
        

        // Sound
        SoundManager.Instance.PlayGunSFX(_currentGunItem.ID);

        
        StartCoroutine(SoundLevelUpByFireRoutine());


        _playerEquip.RefreshHUDAmmoCountText();
    }

    private void Reload()
    {
        if (_currentGunItem.CurrentAmmoCount == _currentGunItem.MagazineCapacity
            || _state != PlayerShootingState.Idle)
            return;

        // 인벤토리에 탄환이 있는지 확인
        if (!_inventory.HasItem(_currentGunItem.BulletId))
            return;

        if (_reloadCoroutine != null)
        {
            StopCoroutine(_reloadCoroutine);
        }

        _reloadCoroutine = StartCoroutine(ReloadRoutine());
    }

    #region Coroutine

    private IEnumerator FireRoutine()
    {
        while (_isFirePressed)
        {
            if (_currentGunItem.CurrentAmmoCount <= 0)
            {
                _state = PlayerShootingState.Idle;
                Reload();
                break;
            }
            else
            {
                if (_state != PlayerShootingState.Idle)
                {
                    _state = PlayerShootingState.Idle;
                    break;
                }

                _state = PlayerShootingState.Shooting;

                Fire();
                yield return new WaitForSeconds(1.0f / _currentGunItem.Rps);
                _state = PlayerShootingState.Idle;
            }
        }
    }

    private IEnumerator ReloadRoutine()
    {
        if (_fireCoroutine != null)
            StopCoroutine(_fireCoroutine);


        _state = PlayerShootingState.Reloading;

        SoundManager.Instance.PlayReloadSFX(true);

        // TODO : 장전 시간 및 UI
        float currentReloadTime = 0f;
        while (currentReloadTime < _currentGunItem.ReloadTime)
        {
            yield return _waitforReloadDelay;
            currentReloadTime += _reloadDelay;
        }

        SoundManager.Instance.PlayReloadSFX(false);

        int maxReloadableAmmoCount = (int)_currentGunItem.MagazineCapacity - _currentGunItem.CurrentAmmoCount;

        // 인벤토리에서 가져올 수 있는 수량 체크 및 아이템 저장
        (int, AmmoItem) reloadable = _inventory.ReloadableAmmoCount(_currentGunItem.BulletId, maxReloadableAmmoCount);

        _currentGunItem.CurrentAmmoCount += reloadable.Item1;
        _currentGunItem.Ammo = reloadable.Item2;

        // ammo count Text
        _playerEquip.RefreshHUDAmmoCountText();

        _state = PlayerShootingState.Idle;

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

    private Vector3 GetMouseWorldPosition()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        Plane groundPlane = new Plane(Vector3.up, _currentGun.MuzzleTransform.position);
        float distance;

        if (groundPlane.Raycast(ray, out distance))
        {
            return ray.GetPoint(distance);
        }

        return Vector3.zero;
    }

    private Vector3 GetFireDirection()
    {
        Vector3 target = GetMouseWorldPosition();

        Vector3 dir = (target - _currentGun.MuzzleTransform.position);

        return dir.normalized;
    }

    #endregion Set Shoot Direction
}
