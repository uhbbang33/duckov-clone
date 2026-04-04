using TMPro;
using UnityEngine;

public class ShopSlotUI : ItemSlotUI
{
    [SerializeField] private GameObject _moneyUI;
    [SerializeField] private TextMeshProUGUI _moneyText;

    private const int _ammoItemCount = 30;
    private const int _defaultItemCount = 1;
    private const float _moneyMultiplier = 1.2f;

    public void SetItemData(int id)
    {
        ItemData itemData = DataManager.Instance.GetItemDataByID(id);
        Item item = itemData.ToItem();

        int itemAmount = (item.Type == ItemType.Ammo)
            ? _ammoItemCount : _defaultItemCount;

        int sellPrice = Mathf.FloorToInt(item.Value * itemAmount * _moneyMultiplier);
        _moneyText.text = sellPrice.ToString();

        _itemSlot.AddItem(item, ref itemAmount);

        SetMoneyUI(true);
    }

    private void SetMoneyUI(bool show)
    {
        _moneyUI.SetActive(show);
    }
}
