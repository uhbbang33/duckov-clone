
public class EquipSlotUI : ItemSlotUI
{
    protected override void Awake()
    {
        base.Awake(); 

        _itemSlot = new ItemSlot();
        _itemSlot.UI = this;
        _itemSlot.Type = SlotType.EQUIP;
    }
}
