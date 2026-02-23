using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private LayerMask _targetLayer;
    [SerializeField] private GameObject _bulletPrefab;

    private GameObject _currentGunObject;
    private GunFireEffectController _currentGunFireEffect;
    private GunItem _currentGun;
    private InputActions _actions;
    private PlayerMove _playerMove;
    private Inventory _inventory;

    public GameObject CurrentGunObject
    {
        get { return _currentGunObject; }
        set { _currentGunObject = value;

            if (_currentGunObject != null)
                _currentGunFireEffect = _currentGunObject.GetComponent<GunFireEffectController>();
            else
                _currentGunFireEffect = null;
        }
    }

    public GunItem CurrentGun
    {
        get { return _currentGun; }
        set { _currentGun = value; }
    }

    private void Start()
    {
        _actions = GetComponent<Player>().Actions;

        _actions.Player.Fire.performed += Fire;

        _playerMove = GetComponent<PlayerMove>();
        _inventory = GetComponent<Inventory>();
    }

    private void OnDisable()
    {
        _actions.Player.Fire.performed -= Fire;
    }

    private void Fire(InputAction.CallbackContext context)
    {
        if (_currentGun == null
            || _currentGunObject == null
            || _playerMove.IsRun
            || _inventory.InventoryIsOpen)
            return;

        if (_currentGun.CurrentAmmoCount <= 0)
        {
            // 장전 (인벤토리에 탄환이 있을 경우)

            return;
        }

        Debug.Log(_currentGun.Name + " shooting!");
        //_currentGun.CurrentAmmoCount -= 1;

        // Sound
        SoundManager.Instance.PlayGunSFX(_currentGun.ID);

        Vector3 muzzlePosition = _currentGunFireEffect.MuzzleTransform.position;

        Vector3 dir = GetFireDirection();

        // bullet
        GameObject bullet = Instantiate(_bulletPrefab, muzzlePosition, Quaternion.identity);
        bullet.GetComponent<Bullet>().Fire(dir, _currentGun.Range);
        
        // raycast
        //Ray ray = new Ray(muzzlePosition, dir);
        //RaycastHit hit;

        //Vector3 endPoint = muzzlePosition + dir * _currentGun.Range;

        //if (Physics.Raycast(ray, out hit, _currentGun.Range, _targetLayer))
        //{
        //    Debug.Log("Hit: " + hit.collider.name);
        //    endPoint = hit.point;
        //}

        //_currentGunFireEffect.Fire(endPoint);
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        Plane groundPlane = new Plane(Vector3.up, _currentGunFireEffect.MuzzleTransform.position);
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

        Vector3 dir = (target - _currentGunFireEffect.MuzzleTransform.position);

        return dir.normalized;
    }

}
