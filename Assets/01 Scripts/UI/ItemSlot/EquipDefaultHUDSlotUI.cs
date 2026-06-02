using TMPro;
using UnityEngine;

public class EquipDefaultHUDSlotUI : DefaultHUDSlotUI
{
    [SerializeField] private GameObject _outline;
    [SerializeField] private GameObject _infoUI;
    [SerializeField] private TextMeshProUGUI _ammoNameText;
    [SerializeField] private TextMeshProUGUI _ammoCountText;

    Inventory _inventory;

    protected override void Start()
    {
        base.Start();

        _inventory = GameManager.Instance.Inventory;
    }

    public void Selected(GunItem gunItem)
    {
        _outline.SetActive(true);
        _infoUI.SetActive(true);

        _ammoNameText.text = gunItem.Ammo.Name;
        RefreshAmmoCountText(gunItem);
    }

    public void Deselected()
    {
        _outline.SetActive(false);
        _infoUI.SetActive(false);
    }

    public void RefreshAmmoCountText(GunItem gunItem)
    {
        if(gunItem == null)
        {
            _ammoCountText.text = string.Empty;
            return;
        }

        int ammoCountInInventory = _inventory.GetAmmoCount(gunItem.AmmoId);

        _ammoCountText.text = gunItem.CurrentAmmoCount.ToString() + "/" + ammoCountInInventory;
    }
}
