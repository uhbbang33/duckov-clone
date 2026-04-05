using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Storage : MonoBehaviour, ISortableContainer, ISaveableContainer
{
    [SerializeField] private Button _sortButton;

    private ItemSlot[] _slots;

    private GameManager _gameManager;

    private int _slotCnt;
    private int _filledSlotCnt;

    private void Start()
    {
        _gameManager = GameManager.Instance;
        _slotCnt = _gameManager.StorageItemSlots.Length;

        _slots = new ItemSlot[_slotCnt];
        for (int i = 0; i < _slotCnt; ++i)
        {
            _slots[i] = new ItemSlot();
            _slots[i].UI = _gameManager.StorageItemSlots[i].GetComponentInChildren<ItemSlotUI>();
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
