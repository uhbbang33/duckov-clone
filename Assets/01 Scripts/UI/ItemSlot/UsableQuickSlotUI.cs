using UnityEngine;

public class UsableQuickSlotUI : ItemSlotUI
{
    [SerializeField] private int _quickSlotNum;

    protected override void Start()
    {
        base.Start();

        _itemSlot = new QuickItemSlot(_quickSlotNum);
        _itemSlot.UI = this;
        _itemSlot.Type = SlotType.QUICKSLOT;
    }

    protected override bool CheckTypeBeforeDrop(ItemSlot startSlot)
    {
        if (startSlot.CurrentItem.Type != ItemType.Medicine
            && startSlot.CurrentItem.Type != ItemType.Food)
            return false;

        if (startSlot.Type != SlotType.INVENTORY)
            return false;

        return true;
    }

}
