using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Inventory : ISortableContainer, ISaveableContainer
{
    private ItemSlot[] _inventorySlots;
    private int _itemCnt;
    private int _slotCnt;
    private float _carryWeight;
    private int _currentMoney;
    private float _maxWeight;

    // key - id, value - slot count
    private Dictionary<uint, int> _inventoryDict;

    // key - id, value - ammo count
    private Dictionary<uint, int> _ammoDict;

    public event Action<float, float> OnWeightChange;
    public event Action<int> OnMoneyChange;
    public event Action<int, int> OnItemCountChange;
    public event Action OnAmmoDictChange;
    public event Action OnInventoryChanged;

    public int CurrentMoney => _currentMoney;
    public int SlotCnt => _slotCnt;
    public int ItemCnt => _itemCnt;


    public Inventory (int slotCount, float maxWeight)
    {
        _slotCnt = slotCount;
        _maxWeight = maxWeight;
        _inventoryDict = new Dictionary<uint, int>();
        _ammoDict = new Dictionary<uint, int>();
        _inventorySlots = new ItemSlot[_slotCnt];

        for (int i = 0; i < _slotCnt; ++i)
        {
            _inventorySlots[i] = new ItemSlot();
            _inventorySlots[i].Type = SlotType.INVENTORY;
            _inventorySlots[i].InventoryIndex = i;
            _inventorySlots[i].InitInventory(this);
        }
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

    public void OnSortCompleted()
    {
        OnInventoryChanged?.Invoke();
    }

    public void FillSlotsWithSaveData(List<int> itemIdList, List<int> gunItemAmmoCountList, List<int> quantityList, List<int> durabilityList, List<int> quickSlotIndexList,int money, DataManager _dataManager, QuickSlotManager _quickSlotManager)
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
            quickSlot.LinkToInventorySlotUI(_inventorySlots[index].UI as InventorySlotUI);
        }

        _currentMoney = money;
        OnMoneyChange?.Invoke(_currentMoney);
    }

    public void LinkSlotUI(ItemSlotUI[] slotUIs)
    {
        if (_slotCnt != slotUIs.Length)
            Debug.LogError("ui와 slot의 개수가 맞지 않습니다");

        for (int i = 0; i < _slotCnt; ++i)
        {
            _inventorySlots[i].UI = slotUIs[i];
        }
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
            ChangeMoney(price, 1, false);
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

        OnAmmoDictChange?.Invoke();
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

        OnAmmoDictChange?.Invoke();
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

        OnItemCountChange?.Invoke(_itemCnt, _slotCnt);
    }

    public void ChangeWeight(bool isAdd, float weightAmount)
    {
        _carryWeight += isAdd ? weightAmount : -weightAmount;
        _carryWeight = Mathf.Round(_carryWeight * 1000f) / 1000f;

        OnWeightChange?.Invoke(_carryWeight, _maxWeight);
    }

    public void ChangeMoney(int itemValue, int itemCount, bool isAdd)
    {
        int amount = itemValue * itemCount;

        _currentMoney += isAdd ? amount : -amount;

        OnMoneyChange?.Invoke(_currentMoney);
    }

    public void ClearInventory()
    {
        foreach(ItemSlot slot in _inventorySlots)
            slot.SubtractItem(slot.Quantity);

        _currentMoney = 0;
    }

    // Quick slot
    public void UseInventoryItem(int itemIndex)
    {
        _inventorySlots[itemIndex].UseItem();
    }
}
