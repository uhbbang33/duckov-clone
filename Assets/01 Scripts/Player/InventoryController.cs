using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryController : MonoBehaviour
{
    private Inventory _inventory;
    private QuickSlotManager _quickSlotManager;
    private SoundManager _soundManager;
    private UIManager _uiManager;

    private InputActions _inputActions;
    private PlayerMove _playerMove;
    private PlayerInteract _playerInteract;
    private PlayerEquip _playerEquip;

    private bool _inventoryToggle;

    public bool InventoryIsOpen { get { return _inventoryToggle; } }

    public event Action<bool> OnInventoryToggle;


    public PlayerInventorySaveData InventorySaveData
    {
        get
        {
            return new PlayerInventorySaveData
            {
                QuickSlotLinkedInventoryIndex = _quickSlotManager.GetLinkedInventoryIndexList(),

                EquipedGunSlotNum = _playerEquip.EquipNum,
                GunSlotItemIDList = _playerEquip.GetGunSlotIDList(),
                GunSlotItemAmmoCountList = _playerEquip.GetGunSlotAmmoCountList(),

                ItemIDList = SaveUtility.GetSlotItemsID(_inventory),
                GunItemAmmoCountList = SaveUtility.GetSlotGunItemsAmmoCount(_inventory),
                QuantityList = SaveUtility.GetSlotsQuantity(_inventory),
                DurabilityList = SaveUtility.GetSlotItemsDurability(_inventory),

                Money = _inventory.CurrentMoney
            };
        }
        set
        {
            _inventory.FillSlotsWithSaveData(
                value.ItemIDList,
                value.GunItemAmmoCountList,
                value.QuantityList,
                value.DurabilityList,
                value.QuickSlotLinkedInventoryIndex,
                value.Money,
                DataManager.Instance,
                _quickSlotManager);

            _playerEquip.FillSlotsWithData(
                value.EquipedGunSlotNum,
                value.GunSlotItemIDList,
                value.GunSlotItemAmmoCountList);
        }
    }


    private void Awake()
    {
        _playerMove = GetComponent<PlayerMove>();
        _playerInteract = GetComponent<PlayerInteract>();
        _playerEquip = GetComponent<PlayerEquip>();
    }

    private void Start()
    {
        _quickSlotManager = QuickSlotManager.Instance;

        _inventory = GameManager.Instance.Inventory;
        _soundManager = SoundManager.Instance;
        _uiManager = UIManager.Instance;

        _inputActions = GameManager.Instance.Actions;
        _inputActions.Player.Inventory.performed += OnInventoryInput;
        _inputActions.Player.Cancel.performed += OnInventoryClose;

        _inventory.OnWeightChange += OnWeightChanged;
        _inventory.OnAmmoDictChange += _playerEquip.RefreshHUDAmmoCountText;

        _playerInteract.OnEnableInteractEvent += OnInventoryCloseBlocked;
        _playerInteract.OnDisableInteractEvent += OnInventoryCloseAllowed;
    }

    private void OnDisable()
    {
        _inputActions.Player.Inventory.performed -= OnInventoryInput;
        _inputActions.Player.Cancel.performed -= OnInventoryClose;

        _inventory.OnWeightChange -= OnWeightChanged;
        _inventory.OnAmmoDictChange -= _playerEquip.RefreshHUDAmmoCountText;

        _playerInteract.OnEnableInteractEvent -= OnInventoryCloseBlocked;
        _playerInteract.OnDisableInteractEvent -= OnInventoryCloseAllowed;
    }

    private void OnInventoryInput(InputAction.CallbackContext context)
    {
        ToggleInventory();
    }

    private void OnInventoryClose(InputAction.CallbackContext context)
    {
        IUICloseable ui = _uiManager.PeekStack();
        if (ui == null || ui.GetType() != typeof(InventoryUI))
            return;

        _uiManager.CloseTopUI();
    }

    public void OnInventoryOpenWithInteractable()
    {
        if (_playerInteract.UI == null || _inventoryToggle)
            return;

        ToggleInventory();
    }

    private void ToggleInventory()
    {
        if (!_inventoryToggle)
        {
            IUICloseable ui = _uiManager.PeekStack();
            if (ui != null)
                return;

            _soundManager.PlaySFXOneShot(SFXName.OpenInventory, 1f);
        }
        SetInventory(!_inventoryToggle);
    }

    private void SetInventory(bool open)
    {
        _inventoryToggle = open;

        if (open)
            _playerMove.StopMove();
        else
            _playerMove.RestartMove();

        if (!open)
            _uiManager.CurrentInfoUI?.HideUI();

        OnInventoryToggle?.Invoke(open);
    }

    public void CloseInventory()
    {
        if (_inventoryToggle)
            SetInventory(false);
    }

    private void OnWeightChanged(float carryWeight, float maxWeight)
    {
        float weightPercentage = (carryWeight / maxWeight) * 100f;

        _playerMove.ChangeSpeed(weightPercentage);
    }

    private void OnInventoryCloseBlocked()
    {
        _inputActions.Player.Inventory.performed -= OnInventoryInput;
    }

    private void OnInventoryCloseAllowed()
    {
        _inputActions.Player.Inventory.performed += OnInventoryInput;
    }

}
