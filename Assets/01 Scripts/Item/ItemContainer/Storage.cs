using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Storage : MonoBehaviour, ISortableContainer, ISaveableContainer
{
    [SerializeField] private Button _sortButton;

    private ItemSlot[] _slots;

    private BunkerManager _bunkerManager;
    private DataManager _dataManager;

    private int _slotCnt;
    private int _filledSlotCnt;

    public StorageSaveData SaveData
    {
        get
        {
            return new StorageSaveData
            {
                ItemIDList = SaveUtility.GetSlotItemsID(this),
                GunItemAmmoCountList = SaveUtility.GetSlotGunItemsAmmoCount(this),
                QuantityList = SaveUtility.GetSlotsQuantity(this),
                DurabilityList = SaveUtility.GetSlotItemsDurability(this)
            };
        }
        set
        {
            FillSlotsWithData(value);
        }
    }

    private void Start()
    {
        _bunkerManager = BunkerManager.Instance;
        _dataManager = DataManager.Instance;
        _slotCnt = _bunkerManager.StorageItemSlots.Length;

        _slots = new ItemSlot[_slotCnt];
        for (int i = 0; i < _slotCnt; ++i)
        {
            _slots[i] = new ItemSlot();
            _slots[i].UI = _bunkerManager.StorageItemSlots[i].GetComponentInChildren<ItemSlotUI>();
            _slots[i].Type = SlotType.STORAGE;
        }

        _sortButton.onClick.AddListener(() => SortUtility.Sort(this));
    }

    // ISortableContainer
    public List<ItemSlot> GetSortableSlots()
    {
        return _slots.ToList();
    }

    // ISaveableContainer
    public IEnumerable<ItemSlot> GetSlots()
    {
        return _slots;
    }

    private void FillSlotsWithData(StorageSaveData data)
    {
        for(int i =0; i < _slots.Length; ++i)
        {
            if (data.ItemIDList[i] == -1)
                continue;

            ItemData itemData = _dataManager.GetItemDataByID(data.ItemIDList[i]);
            Item item = itemData.ToItem();

            if (item.Type == ItemType.Gun)
            {
                (item as GunItem).CurrentAmmoCount = data.GunItemAmmoCountList[i];
            }
            if (item.Type == ItemType.Food || item.Type == ItemType.Medicine)
            {
                (item as UsableItem).CurrentDurability = data.DurabilityList[i];
            }

            int quantity = data.QuantityList[i];

            _slots[i].AddItem(item, ref quantity);
        }
    }

    public void ChangeStorageItemCount(bool isAdd)
    {
        _filledSlotCnt += isAdd ? 1 : -1;
        UIManager.Instance.ChangeStorageItemCountText(_filledSlotCnt, _slotCnt);
    }

    public void AddItemToEmptySlot(Item item, int amount)
    {
        int slotIndex = FindFirstEmptySlot();

        if (slotIndex == -1)
            return;

        _slots[slotIndex].AddItem(item, ref amount);
    }

    public int FindFirstEmptySlot()
    {
        for (int i = 0; i < _slotCnt; ++i)
        {
            if (_slots[i].CurrentItem == null)
            {
                return i;
            }
        }

        return -1;
    }

}
