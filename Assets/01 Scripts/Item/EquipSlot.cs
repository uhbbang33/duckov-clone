using UnityEngine;

public class EquipSlot : ItemSlot
{
    private PlayerEquip _playerEquip;

    public EquipSlot(bool isLeftSlot) : base()
    {
        _slotType = SlotType.EQUIP;
        _playerEquip = GameManager.Instance.PlayerObject.GetComponent<PlayerEquip>();

        if (isLeftSlot)
            _playerEquip.LeftEquipSlot = this;
        else
            _playerEquip.RightEquipSlot = this;
    }

    public override void SubtractItem(int amount = 1)
    {
        base.SubtractItem(amount);

        EquipSlotUI equipUI = _ui as EquipSlotUI;
        if (equipUI.IsLeftSlot)
            _playerEquip.SyncSlotState(true);
        else
            _playerEquip.SyncSlotState(false);
    }

    public override void AddItem(Item item, ref int amount)
    {
        amount = 1;
        base.AddItem(item, ref amount);

        if (_slotType == SlotType.EQUIP)
        {
            EquipSlotUI equipUI = _ui as EquipSlotUI;
            if (equipUI.IsLeftSlot)
                _playerEquip.SyncSlotState(true);
            else
                _playerEquip.SyncSlotState(false);
        }
    }


    public override void UseItem()
    {

    }
}
