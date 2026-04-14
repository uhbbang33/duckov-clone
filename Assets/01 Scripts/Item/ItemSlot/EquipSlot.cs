using UnityEngine;

public class EquipSlot : ItemSlot
{
    private PlayerEquip _playerEquip;

    public EquipSlot(PlayerEquip playerEquip) : base()
    {
        _slotType = SlotType.EQUIP;
        _playerEquip = playerEquip;
    }

    public override void SubtractItem(int amount = 1)
    {
        base.SubtractItem(amount);

        EquipSlotUI equipUI = _ui as EquipSlotUI;
        _playerEquip.SyncSlotState(equipUI.IsLeftSlot);
    }

    public override void AddItem(Item item, ref int amount)
    {
        base.AddItem(item, ref amount);

        EquipSlotUI equipUI = _ui as EquipSlotUI;
        _playerEquip.SyncSlotState(equipUI.IsLeftSlot);
    }


    public override void UseItem()
    {

    }

    public override void UnloadAmmo()
    {
        base.UnloadAmmo();

        (_ui as EquipSlotUI).DefaultHUDSlotUI.RefreshAmmoCountText(_currentItem as GunItem);
    }
}
