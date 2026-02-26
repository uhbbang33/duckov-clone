using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private LayerMask _targetLayer;
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private float _reloadDelay = 0.1f;

    private GameObject _currentGunObject;
    private Gun _currentGun;
    private GunItem _currentGunItem;
    private InputActions _actions;
    private PlayerMove _playerMove;
    private Inventory _inventory;
    private PlayerEquip _playerEquip;

    private bool _isReloading;
    private bool _isFirePressed;

    private Coroutine _fireCoroutine;
    private Coroutine _reloadCoroutine;
    private Coroutine _waitRpsTimeCoroutine;
    private WaitForSeconds _waitforReloadDelay;
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

    public bool IsReloading
    {
        get { return _isReloading; }
    }

    private void Awake()
    {
        _waitforReloadDelay = new WaitForSeconds(_reloadDelay);
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
            return;

        if (context.performed)
        {
            _fireCoroutine = StartCoroutine(FireRoutine());
            Debug.Log("Fire Performed");

            _isFirePressed = true;
        }
        else if (context.canceled)
        {
            Debug.Log("Fire Canceled");

            if (_fireCoroutine != null)
                StopCoroutine(_fireCoroutine);

            _isFirePressed = false;
        }
    }

    private void OnReload(InputAction.CallbackContext context)
    {
        if (_currentGunItem == null
            || _currentGunObject == null
            || _isReloading)
            return;

        Reload();
    }

    private void Fire()
    {
        _currentGunItem.CurrentAmmoCount -= 1;

        Vector3 muzzlePosition = _currentGun.MuzzleTransform.position;
        Vector3 dir = GetFireDirection();

        // bullet
        GameObject bullet = Instantiate(_bulletPrefab, muzzlePosition, Quaternion.identity);
        bullet.GetComponent<Bullet>().Fire(dir, _currentGunItem.Range);


        // Sound
        SoundManager.Instance.PlayGunSFX(_currentGunItem.ID);

        _playerEquip.RefreshHUDAmmoCountText();
    }

    private void Reload()
    {
        if (_currentGunItem.CurrentAmmoCount == _currentGunItem.MagazineCapacity
            || _isReloading)
            return;

        // 인벤토리에 탄환이 있는지 확인
        if (!_inventory.HasItem(_currentGunItem.BulletId))
            return;

        if (_reloadCoroutine != null)
            StopCoroutine(_reloadCoroutine);

        _reloadCoroutine = StartCoroutine(ReloadRoutine());
    }

    public void CutOffReload()
    {
        if (_reloadCoroutine != null)
            StopCoroutine(_reloadCoroutine);

        _isReloading = false;
    }

    #region Coroutine

    private IEnumerator FireRoutine()
    {
        while (true)
        {
            if (_currentGunItem.CurrentAmmoCount <= 0)
            {
                Reload();
                break;
            }
            else
            {
                Fire();
                yield return new WaitForSeconds(1.0f / _currentGunItem.Rps);
            }
        }
    }

    private IEnumerator ReloadRoutine()
    {
        if (_fireCoroutine != null)
            StopCoroutine(_fireCoroutine);

        // sound
        SoundManager.Instance.PlayReloadSFX();

        _isReloading = true;

        Debug.Log("장전 시작!");
        // TODO : 장전 시간 및 UI
        float currentReloadTime = 0f;
        while (currentReloadTime < _currentGunItem.ReloadTime)
        {
            yield return _waitforReloadDelay;
            currentReloadTime += _reloadDelay;
        }
        Debug.Log("장전 끝!");

        int maxReloadableAmmoCount = (int)_currentGunItem.MagazineCapacity - _currentGunItem.CurrentAmmoCount;

        // 인벤토리에서 가져올 수 있는 수량 체크 및 아이템 저장
        (int, AmmoItem) reloadable = _inventory.ReloadableAmmoCount(_currentGunItem.BulletId, maxReloadableAmmoCount);

        _currentGunItem.CurrentAmmoCount += reloadable.Item1;
        _currentGunItem.Ammo = reloadable.Item2;

        // ammo count Text
        _playerEquip.RefreshHUDAmmoCountText();

        if (_isFirePressed)
            _fireCoroutine = StartCoroutine(FireRoutine());

        _isReloading = false;

        yield return null;
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
