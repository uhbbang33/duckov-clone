using UnityEngine;

public class EquipSlotUI : ItemSlotUI
{
    [SerializeField] private EquipDefaultHUDSlotUI _equipDefaultHUDSlotUI;
    [SerializeField] private bool _isLeftSlot;

    private Sprite _pistolIcon;

    public bool IsLeftSlot { get { return _isLeftSlot; } }
    public EquipDefaultHUDSlotUI DefaultHUDSlotUI
    {
        get { return _equipDefaultHUDSlotUI;}
    }

    protected override void Start()
    {
        base.Start();

        _itemSlot = new EquipSlot(_isLeftSlot);
        _itemSlot.UI = this;

        _pistolIcon = _uiManager.PistolIcon;
        SetPistolIcon();
    }

    public override void RefreshUI()
    {
        base.RefreshUI();

        if (_itemSlot.CurrentItem == null)
        {
            SetPistolIcon();
            _equipDefaultHUDSlotUI.RefreshDefaultUHD(false);
        }
        else
        {
            _equipDefaultHUDSlotUI.RefreshDefaultUHD(true, _nameText.text, _countText.text, _iconImage.sprite);
        }
    }

    private void SetPistolIcon()
    {
        _iconImage.sprite = _pistolIcon;
        _uiManager.ChangeImageAlpha(_iconImage, true);
    }

    protected override bool CheckTypeBeforeDrop(ItemSlot startSlot)
    {
        if (startSlot.CurrentItem.Type != ItemType.Gun)
            return false;

        return true;
    }

    public void ChangeLeftGunSlotItem(ItemSlot fromSlot)
    {
        if (fromSlot.CurrentItem.Type != ItemType.Gun)
            return;

        if (_itemSlot.CurrentItem != null)
        {
            SwapItem(fromSlot.UI);
        }
        else
        {
            int amount = fromSlot.Quantity;
            _itemSlot.AddItem(fromSlot.CurrentItem, ref amount);
            fromSlot.SubtractItem();
        }
    }
}
