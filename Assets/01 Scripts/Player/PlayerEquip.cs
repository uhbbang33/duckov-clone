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

    private bool _isLeftSlotActivated = true;
    private int _leftSlotGunId;
    private int _rightSlotGunId;

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
        SyncSlotState(true);
    }

    private void EquipRightSlotGun(InputAction.CallbackContext context)
    {
        _isLeftSlotActivated = false;
        SyncSlotState(false);
    }

    public void SyncSlotState(bool isLeftSlot)
    {
        EquipSlot equipSlot = isLeftSlot ? _leftEquipSlot : _rightEquipSlot;
        bool isActivated = isLeftSlot ? _isLeftSlotActivated : !_isLeftSlotActivated;

        if (equipSlot == null)
            return;

        int gunId = equipSlot.CurrentItem == null ? 0 : (int)equipSlot.CurrentItem.ID;

        if (isLeftSlot)
            _leftSlotGunId = gunId;
        else
            _rightSlotGunId = gunId;

        if (!isActivated)
            return;

        ApplyEquipState(isLeftSlot, equipSlot);
    }

    private void ApplyEquipState(bool isLeftSlot, EquipSlot equipSlot)
    {
        bool hasItem = equipSlot.CurrentItem != null;

        if (_gunObject != null && hasItem && _currentSelectedSlot != equipSlot)
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
            ui.DefaultHUDSlotUI.Selected();
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
