
using TMPro;
using UnityEngine;

public class EquipSlotUI : ItemSlotUI
{
    [SerializeField] private TextMeshProUGUI _equipNameText;

    private Sprite _pistolIcon;

    protected override void Start()
    {
        base.Start();

        _itemSlot = new ItemSlot();
        _itemSlot.UI = this;
        _itemSlot.Type = SlotType.EQUIP;

        _pistolIcon = _uiManager.PistolIcon;

        SetPistolIcon();
    }

    public override void RefreshUI()
    {
        base.RefreshUI();

        if (_itemSlot.CurrentItem == null)
        {
            SetPistolIcon();
            _equipNameText.text = "ÃÑ±â";
        }
        else
            _equipNameText.text = _itemSlot.CurrentItem.Name;
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
}
