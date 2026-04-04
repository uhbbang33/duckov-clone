using TMPro;
using UnityEngine;

public class ShopSlotUI : ItemSlotUI
{
    [SerializeField] private GameObject _moneyUI;
    [SerializeField] private TextMeshProUGUI _moneyText;

    private const int _ammoItemCount = 30;
    private const int _defaultItemCount = 1;

    public void SetItemData(int id)
    {
        ItemData itemData = DataManager.Instance.GetItemDataByID(id);
        Item item = itemData.ToItem();

        int itemAmount = (item.Type == ItemType.Ammo)
            ? _ammoItemCount : _defaultItemCount;

        _itemSlot.AddItem(item, ref itemAmount);
        _moneyText.text = item.Value.ToString();

        SetMoneyUI(true);
    }

    private void SetMoneyUI(bool show)
    {
        _moneyUI.SetActive(show);
    }
}
