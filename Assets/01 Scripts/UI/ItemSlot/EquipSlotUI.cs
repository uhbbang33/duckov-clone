using UnityEngine;

public class EquipSlotUI : ItemSlotUI
{
    [SerializeField] private DefaultHUDSlotUI _defaultHUDSlotUI;
    [SerializeField] private bool _isLeftSlot;

    private Sprite _pistolIcon;

    public bool IsLeftSlot { get { return _isLeftSlot; } }

    protected override void Start()
    {
        base.Start();

        _itemSlot = new EquipSlot();
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
            _defaultHUDSlotUI.RefresuhDefaultUHD(false);
        }
        else
        {
            _defaultHUDSlotUI.RefresuhDefaultUHD(true, _nameText.text, _countText.text, _iconImage.sprite);
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
            _itemSlot.AddItem(fromSlot.CurrentItem);
            fromSlot.SubtractItem();
        }
    }
}
