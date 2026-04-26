using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShopSlotUI : ItemSlotUI
{
    [SerializeField] private GameObject _moneyUI;
    [SerializeField] private TextMeshProUGUI _moneyText;

    private int _sellPrice;

    private const int _ammoItemCount = 30;
    private const int _defaultItemCount = 1;
    private const float _priceMultiplier = 1.2f;

    public void SetItemData(int id)
    {
        ItemData itemData = DataManager.Instance.GetItemDataByID(id);
        Item item = itemData.ToItem();

        int itemAmount = (item.Type == ItemType.Ammo)
            ? _ammoItemCount : _defaultItemCount;

        _sellPrice = Mathf.FloorToInt(item.Value * itemAmount * _priceMultiplier);
        _moneyText.text = _sellPrice.ToString();

        _itemSlot.AddItem(item, ref itemAmount);

        SetMoneyUI(true);
    }

    private void SetMoneyUI(bool show)
    {
        _moneyUI.SetActive(show);
    }

    protected override void OpenSlotMenu()
    {
        return;
    }

    public override void OnDrop(PointerEventData eventData)
    {
        return;
    }

    protected override void OnDoubleClick()
    {
        base.OnDoubleClick();

        if(_inventory.TryBuyItem(_itemSlot.CurrentItem, _itemSlot.Quantity, _sellPrice))
        {
            PlaySellBuySound();
        }
    }
}
