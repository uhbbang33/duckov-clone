using UnityEngine;

public class Shop : MonoBehaviour
{
    private ItemSlot[] _slots;
    private int _slotCnt;
    DataManager _dataManager;
    GameManager _gameManager;

    private void Start()
    {
        _dataManager = DataManager.Instance;
        _gameManager = GameManager.Instance;

        _slotCnt = _gameManager.ShopItemSlots.Length;

        _slots = new ItemSlot[_slotCnt];

        for (int i = 0; i < _slotCnt; ++i)
        {
            _slots[i] = new ItemSlot();
            _slots[i].UI = _gameManager.ShopItemSlots[i].GetComponentInChildren<ShopSlotUI>();
            _slots[i].Type = SlotType.SHOP;
        }

        foreach(var data in _dataManager.ShopItemList.ShopItemDatas)
        {
            (_slots[data.SlotNum].UI as ShopSlotUI).SetItemData((int)data.Id);
        }
    }

}
