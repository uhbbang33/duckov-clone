using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using UnityEngine.UI;
using System.Linq;

public class Inventory : MonoBehaviour, ISortableContainer, ISaveableContainer
{
    [SerializeField] private GameObject _inventoryUI;
    [SerializeField] private GameObject[] _slotObject;
    [SerializeField] private Button _sortButton;
    [SerializeField] private float _maxWeight;

    private UIManager _uiManager;
    private DataManager _dataManager;
    private QuickSlotManager _quickSlotManager;
    private InputActions _inputActions;
    private PlayerMove _playerMove;
    private PlayerInteract _playerInteract;
    private PlayerEquip _playerEquip;
    private PlayerShooting _playerShooting;

    private ItemSlot[] _inventorySlots;
    private int _itemCnt;
    private int _slotCnt;
    private bool _inventoryToggle;
    private float _carryWeight;
    private int _currentMoney;

    // key - id, value - slot count
    private Dictionary<uint, int> _inventoryDict;

    // key - id, value - ammo count
    private Dictionary<uint, int> _ammoDict;

    public event Action<float, float> OnWeightChange;

    public bool InventoryIsOpen { get { return _inventoryToggle; } }

    public PlayerInventorySaveData InventorySaveData
    {
        get
        {
            return new PlayerInventorySaveData
            {
                EquipedGunSlotNum = _playerEquip.EquipNum,
                QuickSlotLinkedInventoryIndex = _quickSlotManager.GetLinkedInventoryIndexList(),

                ItemIDList = SaveUtility.GetSlotItemsID(this),
                GunItemAmmoCountList = SaveUtility.GetSlotGunItemsAmmoCount(this),
                QuantityList = SaveUtility.GetSlotsQuantity(this),
                DurabilityList = SaveUtility.GetSlotItemsDurability(this),

                Money = _currentMoney
            };
        }
        set
        {
            FillInventorySlotsWithData(
                value.ItemIDList, 
                value.GunItemAmmoCountList,
                value.QuantityList,
                value.DurabilityList,
                value.QuickSlotLinkedInventoryIndex);

            _currentMoney = value.Money;
            _uiManager.ChangeInventoryMoneyText(_currentMoney);

            _playerEquip.EquipNum = value.EquipedGunSlotNum;
        }
    }

    private void Awake()
    {
        _uiManager = UIManager.Instance;
        _dataManager = DataManager.Instance;
        _quickSlotManager = QuickSlotManager.Instance;

        _playerMove = GetComponent<PlayerMove>();
        _playerInteract = GetComponent<PlayerInteract>();
        _playerEquip = GetComponent<PlayerEquip>();
        _playerShooting = GetComponent<PlayerShooting>();

        _inventoryUI.SetActive(false);
        _inventoryDict = new Dictionary<uint, int>();
        _ammoDict = new Dictionary<uint, int>();

        _slotCnt = _slotObject.Length;

        _inventorySlots = new ItemSlot[_slotCnt];
    }

    private void Start()
    {
        for (int i = 0; i < _slotCnt; ++i)
        {
            ItemSlotUI slotUI = _slotObject[i].GetComponentInChildren<ItemSlotUI>();

            _inventorySlots[i] = new ItemSlot();
            _inventorySlots[i].UI = slotUI;
            _inventorySlots[i].Type = SlotType.INVENTORY;
            _inventorySlots[i].InventoryIndex = i;
        }


        _inputActions = GetComponent<Player>().Actions;
        _inputActions.Player.Inventory.performed += OnInventory;
        _inputActions.Player.Cancel.performed += OnInventoryClose;
        // TODO : Inventory에 player가 가지고 있는 물품 넣기 (저장)

        _playerInteract.OnEnableInteractEvent += OnInventoryCloseBlocked;
        _playerInteract.OnDisableInteractEvent += OnInventoryCloseAllowed;

        _sortButton.onClick.AddListener(() => SortUtility.Sort(this));
    }

    private void OnDisable()
    {
        _inputActions.Player.Inventory.performed -= OnInventory;
        _inputActions.Player.Cancel.performed -= OnInventoryClose;

        _playerInteract.OnEnableInteractEvent -= OnInventoryCloseBlocked;
        _playerInteract.OnDisableInteractEvent -= OnInventoryCloseAllowed;
    }

    // ISortableContainer
    public List<ItemSlot> GetSortableSlots()
    {
        return _inventorySlots.ToList();
    }

    // ISaveableContainer
    public IEnumerable<ItemSlot> GetSlots()
    {
        return _inventorySlots;
    }

    private void FillInventorySlotsWithData(List<int> itemIdList, List<int> gunItemAmmoCountList, List<int> quantityList, List<int> durabilityList, List<int> quickSlotIndexList)
    {
        for (int i = 0; i < _inventorySlots.Length; ++i)
        {
            if (itemIdList[i] == -1)
                continue;

            ItemData data = _dataManager.GetItemDataByID(itemIdList[i]);
            Item item = data.ToItem();

            if(item.Type == ItemType.Gun)
            {
                (item as GunItem).CurrentAmmoCount = gunItemAmmoCountList[i];
            }
            
            if(item.Type == ItemType.Food || item.Type == ItemType.Medicine)
            {
                (item as UsableItem).CurrentDurability = durabilityList[i];
            }

            int quantity = quantityList[i];

            _inventorySlots[i].AddItem(item, ref quantity);
        }

        for (int i = 0; i < quickSlotIndexList.Count; ++i)
        {
            int index = quickSlotIndexList[i];
            if (index == -1)
                continue;

            QuickSlot quickSlot = _quickSlotManager.GetQuickSlotByIndex(i);
            quickSlot.LinkToInventorySlotUI(_inventorySlots[index].UI);
        }
    }

    public void OnSortCompleted()
    {
        RefreshQuickSlots();
    }

    private void RefreshQuickSlots()
    {

    }

    private void OnInventory(InputAction.CallbackContext context)
    {
        OpenInventory();
    }

    public void OnInventoryOpenWithInteractable()
    {
        if (_playerInteract.UI == null || _inventoryToggle)
            return;

        OpenInventory();
    }

    private void OnInventoryClose(InputAction.CallbackContext context)
    {
        if (!_inventoryToggle)
            return;

        _inventoryUI.SetActive(false);

        _playerMove.RestartMove();

        _inventoryToggle = false;

        _uiManager.DefaultUHDShowToggle(true);
        _uiManager.ShowCursor(false);
        _uiManager.PlayerCanvasShowToggle(true);
    }

    private void OpenInventory()
    {
        // TODO : 상호작용 UI도 없어져야함
        // TODO : UIManager에서 
        // TODO : Player HP, SP Bar hide
        _inventoryToggle = !_inventoryToggle;

        _inventoryUI.SetActive(_inventoryToggle);

        if (_inventoryToggle)
        {
            _playerMove.StopMove();

            _uiManager.DefaultUHDShowToggle(false);
            _uiManager.ShowCursor(true);
        }
        else
        {
            _playerMove.RestartMove();

            _uiManager.DefaultUHDShowToggle(true);
            _uiManager.ShowCursor(false);
        }

        OnWeightChange?.Invoke(_carryWeight, _maxWeight);

        _uiManager.ChangeInventoryItemCountText(_itemCnt, _slotCnt);
        _uiManager.PlayerCanvasShowToggle(false);

        _playerShooting.IsFirePressed = false;
    }


    private void OnInventoryCloseBlocked()
    {
        _inputActions.Player.Inventory.performed -= OnInventory;
    }

    private void OnInventoryCloseAllowed()
    {
        _inputActions.Player.Inventory.performed += OnInventory;
    }

    public bool TryAddItem(Item item, ref int amount)
    {
        // 인벤토리에 같은 아이템이 있을 경우
        if (_inventoryDict.ContainsKey(item.ID))
        {
            int itemInInventoryCount = _inventoryDict[item.ID];

            for (int i = 0; i < _slotCnt; ++i)
            {
                if (_inventorySlots[i].CurrentItem == null)
                    continue;

                if (_inventorySlots[i].CurrentItem.ID == item.ID)
                {
                    _inventorySlots[i].AddItem(item, ref amount);

                    if (amount == 0)
                        return true;

                    itemInInventoryCount -= 1;
                    if (itemInInventoryCount == 0)
                        break;
                }
            }
        }

        // 같은 아이템이 없어서 빈 슬롯에 아이템을 넣는 경우
        if (TryAddItemToEmptySlot(item, amount))
        {
            amount = 0;
            return true;
        }

        return false;
    }

    private bool CanAddItem(uint itemId, int quantity)
    {
        // 빈공간이 있으면 무조건 아이템 추가 가능
        if (FindFirstEmptySlot() != -1)
            return true;

        // 빈공간도 없고 인벤토리에 같은 아이템이 없다면 return false
        if (!_inventoryDict.ContainsKey(itemId))
            return false;

        int itemInInventoryCount = _inventoryDict[itemId];
        int remain = quantity;

        for (int i = 0; i < _slotCnt; ++i)
        {
            if (_inventorySlots[i].CurrentItem == null)
                continue;

            if (_inventorySlots[i].CurrentItem.ID == itemId)
            {
                int canAddCount = (int)_inventorySlots[i].CurrentItem.MaxStackSize - _inventorySlots[i].Quantity;

                remain -= canAddCount;
                if (remain <= 0)
                    return true;

                itemInInventoryCount -= 1;
                if (itemInInventoryCount == 0)
                    break;
            }
        }

        return false;
    }

    public bool TryBuyItem(Item item, int quantity, int price)
    {
        // 1. 재화 있는지 확인
        if (_currentMoney < price)
            return false;

        // 2. 빈자리 있는지 확인
        if (CanAddItem(item.ID, quantity))
        {
            ChangeMoney(price, quantity, false);
            TryAddItem(item, ref quantity);
            return true;
        }

        return false;
    }

    public bool TryAddItemToEmptySlot(Item item, int amount)
    {
        int slotIndex = FindFirstEmptySlot();

        if (slotIndex == -1)
            return false;

        _inventorySlots[slotIndex].AddItem(item, ref amount);
        return true;
    }

    public int FindFirstEmptySlot()
    {
        for (int i = 0; i < _slotCnt; ++i)
            if (_inventorySlots[i].CurrentItem == null)
                return i;

        return -1;
    }

    public (int, AmmoItem) ReloadableAmmoCount(uint id, int max)
    {
        int reloadableAmmoCount = 0;
        AmmoItem ammoItem = null;

        for (int i = 0; i < _slotCnt; ++i)
        {
            if (!HasItem(id))
                break;

            if (_inventorySlots[i].CurrentItem == null)
                continue;

            if (_inventorySlots[i].CurrentItem.ID == id)
            {
                int amount = 0;

                if (max <= reloadableAmmoCount + _inventorySlots[i].Quantity)
                    amount = max - reloadableAmmoCount;
                else
                    amount = _inventorySlots[i].Quantity;

                if (ammoItem == null)
                    ammoItem = _inventorySlots[i].CurrentItem as AmmoItem;

                reloadableAmmoCount += amount;

                _inventorySlots[i].SubtractItem(amount);
            }

            if (reloadableAmmoCount == max)
                break;
        }

        return (reloadableAmmoCount, ammoItem);
    }

    public bool HasItem(uint id)
    {
        if (_inventoryDict.ContainsKey(id))
            return true;

        return false;
    }

    public void AddToInventoryDictByID(uint id)
    {
        if (_inventoryDict.ContainsKey(id))
            _inventoryDict[id] += 1;
        else
            _inventoryDict.Add(id, 1);

        ChangeItemCount(true);
    }

    public void AddToAmmoDictByID(uint id, int ammoCount)
    {
        if (_ammoDict.ContainsKey(id))
            _ammoDict[id] += ammoCount;
        else
            _ammoDict.Add(id, ammoCount);

        _playerEquip.RefreshHUDAmmoCountText();
    }

    public void RemoveItemSlot(uint id)
    {
        if (!_inventoryDict.ContainsKey(id))
            return;

        _inventoryDict[id] -= 1;

        if (_inventoryDict[id] == 0)
            _inventoryDict.Remove(id);

        ChangeItemCount(false);
    }

    public void ReduceAmmoCount(uint id, int count)
    {
        if (!_ammoDict.ContainsKey(id))
            return;

        _ammoDict[id] -= count;

        if (_ammoDict[id] <= 0)
            _ammoDict.Remove(id);

        _playerEquip.RefreshHUDAmmoCountText();
    }

    public int GetAmmoCount(uint id)
    {
        if (_ammoDict.ContainsKey(id))
            return _ammoDict[id];
        else
            return 0;
    }

    public void ChangeItemCount(bool isAdd)
    {
        if (isAdd)
            ++_itemCnt;
        else
            --_itemCnt;

        _uiManager.ChangeInventoryItemCountText(_itemCnt, _slotCnt);
    }

    public void ChangeWeight(bool isAdd, float weightAmount)
    {
        _carryWeight += isAdd ? weightAmount : -weightAmount;
        _carryWeight = Mathf.Round(_carryWeight * 1000f) / 1000f;

        OnWeightChange?.Invoke(_carryWeight, _maxWeight);

        ChangePlayerSpeed();
    }

    private void ChangePlayerSpeed()
    {
        float weightPercentage = (_carryWeight / _maxWeight) * 100f;

        _playerMove.ChangeSpeed(weightPercentage);
    }

    public void ChangeMoney(int itemValue, int itemCount, bool isAdd)
    {
        int amount = itemValue * itemCount;

        _currentMoney += isAdd ? amount : -amount;

        _uiManager.ChangeInventoryMoneyText(_currentMoney);
    }

    // Quick slot
    public void UseInventoryItem(int itemIndex)
    {
        _inventorySlots[itemIndex].UseItem();
    }

}
