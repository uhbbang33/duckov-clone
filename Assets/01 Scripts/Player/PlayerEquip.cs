using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerEquip : MonoBehaviour
{
    [SerializeField] private Transform _rightHandTransform;

    private Animator _anim;
    private GameObject _gunObject;
    private InputActions _inputActions;

    private bool _isLeftSlotActivated = true;
    private int _leftSlotGunId;
    private int _rightSlotGunId;

    private const string _raiseArm = "RaiseArm";
    private const string _changeWeapon = "ChangeWeapon";


    public int LeftSlotGundId
    {
        get { return _leftSlotGunId; }
        set
        {
            _leftSlotGunId = value;

            if (_isLeftSlotActivated)
            {
                if(value == 0)
                    UnequipGun();
                else
                    EquipGun(true);
            }
        }
    }

    public int RightSlotGundId
    {
        get { return _rightSlotGunId; }
        set
        {
            _rightSlotGunId = value;

            if (!_isLeftSlotActivated)
            {
                if (value == 0)
                    UnequipGun();
                else
                    EquipGun(false);
            }
        }
    }

    private void Start()
    {
        _anim = GetComponent<Animator>();
        _inputActions = GetComponent<Player>().Actions;

        _inputActions.Player.LeftWeapon.performed += EquipLeftSlotGun;
        _inputActions.Player.RightWeapon.performed += EquipRightSlotGun;
    }

    private void OnDisable()
    {
        _inputActions.Player.LeftWeapon.performed -= EquipLeftSlotGun;
        _inputActions.Player.RightWeapon.performed -= EquipRightSlotGun;
    }

    private void EquipLeftSlotGun(InputAction.CallbackContext context)
    {
        _isLeftSlotActivated = true;
    }

    private void EquipRightSlotGun(InputAction.CallbackContext context)
    {
        _isLeftSlotActivated = false;
    }

    private void EquipGun(bool isLeftSlot)
    {
        if (_gunObject != null)
        {
            _anim.SetTrigger(_changeWeapon);
        }
        _anim.SetBool(_raiseArm, true);
    }

    private void UnequipGun()
    {
        _anim.SetBool(_raiseArm, false);
    }


    #region Animation Event

    public void CreateGunPrefab()
    {
        int gunId = _isLeftSlotActivated ? _leftSlotGunId : _rightSlotGunId;

        if (gunId == GunId.Mp7Id)
            _gunObject = Instantiate(GameResources.Instance.Mp7Prefab, _rightHandTransform);
        else if (gunId == GunId.M700Id)
            _gunObject = Instantiate(GameResources.Instance.M700Prefab, _rightHandTransform);
        else if (gunId == GunId.GlockId)
            _gunObject = Instantiate(GameResources.Instance.GlockPrefab, _rightHandTransform);
        else
            return;
    }

    public void DestroyPefab()
    {
        if (_gunObject != null)
            Destroy(_gunObject);
    }

    #endregion
}
