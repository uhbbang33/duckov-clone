
public class EquipSlot : ItemSlot
{
    private PlayerEquip _playerEquip;

    public EquipSlot() : base()
    {
        _slotType = SlotType.EQUIP;
        _playerEquip = GameManager.Instance.PlayerObject.GetComponent<PlayerEquip>();
    }

    public override void SubtractItem(int amount = 1)
    {
        base.SubtractItem(amount);

        EquipSlotUI equipUI = _ui as EquipSlotUI;
        if (equipUI.IsLeftSlot)
            _playerEquip.LeftSlotGundId = 0;
        else
            _playerEquip.RightSlotGundId = 0;
    }

    public override int AddItem(Item item, int amount = 1)
    {
        if (amount == 0) return amount;

        if (_currentItem == null)
        {
            if (_slotType == SlotType.EQUIP)
            {
                EquipSlotUI equipUI = _ui as EquipSlotUI;
                if (equipUI.IsLeftSlot)
                    _playerEquip.LeftSlotGundId = (int)item.ID;
                else
                    _playerEquip.RightSlotGundId = (int)item.ID;
            }
        }

        return base.AddItem(item, amount);
    }


    public override void UseItem()
    {

    }
}
