using UnityEngine;

public class EquipSlot : ItemSlot
{
    private PlayerEquip _playerEquip;

    public EquipSlot(bool isLeftSlot) : base()
    {
        _slotType = SlotType.EQUIP;
        _playerEquip = GameManager.Instance.PlayerObject.GetComponent<PlayerEquip>();

        if (isLeftSlot && _playerEquip.LeftEquipSlot == null)
            _playerEquip.LeftEquipSlot = this;
        else if(_playerEquip.RightEquipSlot == null)
            _playerEquip.RightEquipSlot = this;
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
