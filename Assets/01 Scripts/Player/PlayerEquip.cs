using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerEquip : MonoBehaviour
{
    [SerializeField] private Transform _rightHandTransform;

    private Animator _anim;
    private GameObject _gunObject;
    private InputActions _inputActions;
    private EquipSlot _leftEquipSlot;
    private EquipSlot _rightEquipSlot;
    private EquipSlot _currentSelectedSlot;
    private PlayerShooting _playerShooting;
    private PoolManager _poolManager;

    private bool _isLeftSlotActivated = true;
    private bool _isRightSlotActivated = false;
    private int _leftSlotGunId;
    private int _rightSlotGunId;
    private uint _currentGunPoolId;

    private const string _raiseArm = "RaiseArm";
    private const string _changeWeapon = "ChangeWeapon";

    
    public EquipSlot LeftEquipSlot
    {
        get { return _leftEquipSlot; }
        set { _leftEquipSlot = value; }
    }

    public EquipSlot RightEquipSlot
    {
        get { return _rightEquipSlot; }
        set { _rightEquipSlot = value; }
    }
    public int EquipNum
    {
        get
        {
            if (_isLeftSlotActivated)
                return 1;
            else return 2;
        }
        set
        {
            if (value == 1)
            {
                _isLeftSlotActivated = true;
                _isRightSlotActivated = false;
            }
            else if(value == 2)
            {
                _isLeftSlotActivated = false;
                _isRightSlotActivated = true;
            }
        }
    }

    private void Awake()
    {
        _leftEquipSlot = null;
        _rightEquipSlot = null;
        _currentSelectedSlot = null;
    }

    private void Start()
    {
        _anim = GetComponent<Animator>();
        _playerShooting = GetComponent<PlayerShooting>();
        _inputActions = GetComponent<Player>().Actions;

        _inputActions.Player.LeftWeapon.performed += EquipLeftSlotGun;
        _inputActions.Player.RightWeapon.performed += EquipRightSlotGun;

        _poolManager = PoolManager.Instance;
    }

    private void OnDisable()
    {
        _inputActions.Player.LeftWeapon.performed -= EquipLeftSlotGun;
        _inputActions.Player.RightWeapon.performed -= EquipRightSlotGun;
    }

    private void EquipLeftSlotGun(InputAction.CallbackContext context)
    {
        if (_isLeftSlotActivated
            || _playerShooting.FireState != PlayerFireState.Idle)
            return;

        _isLeftSlotActivated = true;
        _isRightSlotActivated = false;
        SyncSlotState(true);
    }

    private void EquipRightSlotGun(InputAction.CallbackContext context)
    {
        if (_isRightSlotActivated
             || _playerShooting.FireState != PlayerFireState.Idle)
            return;

        _isLeftSlotActivated = false;
        _isRightSlotActivated = true;
        SyncSlotState(false);
    }

    public void SyncSlotState(bool isLeftSlot)
    {
        EquipSlot equipSlot = isLeftSlot ? _leftEquipSlot : _rightEquipSlot;
        bool isActivated = isLeftSlot ? _isLeftSlotActivated : _isRightSlotActivated;

        if (!isActivated)
            return;

        if (equipSlot == null)
            return;

        int gunId = equipSlot.CurrentItem == null ? 0 : (int)equipSlot.CurrentItem.ID;

        if (isLeftSlot)
            _leftSlotGunId = gunId;
        else
            _rightSlotGunId = gunId;


        ApplyEquipState(equipSlot);
    }

    private void ApplyEquipState(EquipSlot equipSlot)
    {
        _playerShooting.FireState = PlayerFireState.ChangeGun;

        bool hasItem = equipSlot.CurrentItem != null;

        if (_gunObject != null && hasItem)
        {
            ChangeGun(equipSlot);
            return;
        }

        if (!hasItem)
        {
            UnequipGun();
            return;
        }

        _currentSelectedSlot = equipSlot;
        EquipGun();
    }

    private void ChangeGun(EquipSlot equipSlot)
    {
        RefreshHUDSelection(equipSlot);

        _anim.SetTrigger(_changeWeapon);
        _anim.SetBool(_raiseArm, true);

        _currentSelectedSlot = equipSlot;
    }

    private void EquipGun()
    {
        RefreshHUDSelection(_currentSelectedSlot);
        _anim.SetBool(_raiseArm, true);
    }

    private void UnequipGun()
    {
        DeselectDefaultHUD(_currentSelectedSlot);
        _anim.SetBool(_raiseArm, false);
    }

    private void SelectDefaultHUD(EquipSlot slot)
    {
        if (slot?.UI is EquipSlotUI ui)
            ui.DefaultHUDSlotUI.Selected(slot.CurrentItem as GunItem);
    }

    private void DeselectDefaultHUD(EquipSlot slot)
    {
        if (slot?.UI is EquipSlotUI ui)
            ui.DefaultHUDSlotUI.Deselected();
    }

    // 여러번 총기 change시 InfoUI 호출 순서 겹치는 오류 방지
    private void RefreshHUDSelection(EquipSlot slot)
    {
        DeselectDefaultHUD(_leftEquipSlot);
        DeselectDefaultHUD(_rightEquipSlot);

        if (slot != null)
            SelectDefaultHUD(slot);

        _currentSelectedSlot = slot;
    }

    // TODO
    public void RefreshHUDAmmoCountText()
    {
        if (_currentSelectedSlot == null)
            return;

        (_currentSelectedSlot.UI as EquipSlotUI).DefaultHUDSlotUI.RefreshAmmoCountText(_currentSelectedSlot.CurrentItem as GunItem);
    }

    public void CurrentGunSlotRefreshUI()
    {
        _currentSelectedSlot.UI.RefreshUI();
    }

    #region Animation Event

    // TODO : 미리 생성 후 불러오는 방식으로
    public void CreateGunPrefab()
    {
        int gunId = _isLeftSlotActivated ? _leftSlotGunId : _rightSlotGunId;

        if (gunId == GunId.Mp7Id)
            _currentGunPoolId = PoolId.Mp7;
        else if (gunId == GunId.M700Id)
            _currentGunPoolId = PoolId.M700;
        else if (gunId == GunId.GlockId)
            _currentGunPoolId = PoolId.Glock;
        else
            return;

        _gunObject = _poolManager.GetObject(_currentGunPoolId, _rightHandTransform, true);

        _playerShooting.CurrentGunObject = _gunObject;
        _playerShooting.CurrentGunItem = _currentSelectedSlot.CurrentItem as GunItem;

        _playerShooting.FireState = PlayerFireState.Idle;
    }

    public void DestroyPefab()
    {
        if (_gunObject == null)
            return;

        _poolManager.ReturnObject(_currentGunPoolId, _gunObject);

        _playerShooting.CurrentGunObject = null;
        _playerShooting.CurrentGunItem = null;

        _playerShooting.FireState = PlayerFireState.Idle;
    }

    #endregion
}
