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
    private Coroutine _coroutine;
    private WaitForSeconds _waitforReloadDelay;

    public GameObject CurrentGunObject
    {
        get { return _currentGunObject; }
        set { _currentGunObject = value;

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

    private void Awake()
    {
        _waitforReloadDelay = new WaitForSeconds(_reloadDelay);
    }

    private void Start()
    {
        _actions = GetComponent<Player>().Actions;

        _actions.Player.Fire.performed += OnFire;
        _actions.Player.Reload.performed += OnReload;

        _playerMove = GetComponent<PlayerMove>();
        _inventory = GetComponent<Inventory>();
    }

    private void OnDisable()
    {
        _actions.Player.Fire.performed -= OnFire;
        _actions.Player.Reload.performed -= OnReload;
    }

    private void OnFire(InputAction.CallbackContext context)
    {
        if (_currentGunItem == null
            || _currentGunObject == null
            || _playerMove.IsRun
            || _inventory.InventoryIsOpen)
            return;

        if (_currentGunItem.CurrentAmmoCount <= 0)
        {
            Reload();
            return;
        }

        //Debug.Log(_currentGunItem.Name + " shooting!");
        _currentGunItem.CurrentAmmoCount -= 1;

        Vector3 muzzlePosition = _currentGun.MuzzleTransform.position;
        Vector3 dir = GetFireDirection();

        // bullet
        GameObject bullet = Instantiate(_bulletPrefab, muzzlePosition, Quaternion.identity);
        bullet.GetComponent<Bullet>().Fire(dir, _currentGunItem.Range);


        // Sound
        SoundManager.Instance.PlayGunSFX(_currentGunItem.ID);
    }

    private void OnReload(InputAction.CallbackContext context)
    {
        if (_currentGunItem == null
            || _currentGunObject == null)
            return;

        Reload();
    }

    private void Reload()
    {
        if (_currentGunItem.CurrentAmmoCount == _currentGunItem.MagazineCapacity)
            return;

        // 인벤토리에 탄환이 있는지 확인
        if (!_inventory.HasItem(_currentGunItem.BulletId))
            return;

        if (_coroutine != null)
            StopCoroutine(_coroutine);
        
        _coroutine = StartCoroutine(ShowReloadUIRoutine());
    }

    private IEnumerator ShowReloadUIRoutine()
    {
        // 장전하는동안 걷기이외의 행동을 할 경우 장전 중단

        // 장전 효과음

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

        // 인벤토리에서 가져올 수 있는 수량 체크
        int reloadableAmmoCount = _inventory.ReloadableAmmoCount(_currentGunItem.BulletId, maxReloadableAmmoCount);

        // 장전 시간이 끝난 후 실제 ammoCount 변화
        _currentGunItem.CurrentAmmoCount += reloadableAmmoCount;

        yield return null;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        Plane groundPlane = new Plane(Vector3.up, _currentGun.MuzzleTransform.position);
        float distance;

        if(groundPlane.Raycast(ray, out distance))
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

}
