using UnityEngine;

public class Storage : MonoBehaviour
{
    private ItemSlot[] _slots;
    private StorageUI _ui;

    private GameManager _gameManager;

    private int _slotCnt;
    private int _filledSlotCnt;

    private void Awake()
    {
        _ui = GetComponent<StorageUI>();
    }

    private void Start()
    {
        _gameManager = GameManager.Instance;
        _slotCnt = _gameManager.StorageItemSlots.Length;

        _slots = new ItemSlot[_slotCnt];
        for(int i =0; i < _slotCnt; ++i)
        {
            _slots[i] = new ItemSlot();
            _slots[i].UI = _gameManager.StorageItemSlots[i].GetComponentInChildren<ItemSlotUI>();
            _slots[i].Type = SlotType.STORAGE;
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
