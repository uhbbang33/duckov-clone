using TMPro;
using UnityEngine;

public class EquipDefaultHUDSlotUI : DefaultHUDSlotUI
{
    [SerializeField] private GameObject _outline;
    [SerializeField] private GameObject _infoUI;
    [SerializeField] private TextMeshProUGUI _ammoNameText;
    [SerializeField] private TextMeshProUGUI _ammoCountText;
    
    public void Selected(GunItem gunItem)
    {
        _outline.SetActive(true);
        _infoUI.SetActive(true);

        _ammoNameText.text = gunItem.AmmoName;
        RefreshAmmoCountText(gunItem);
    }

    public void Deselected()
    {
        _outline.SetActive(false);
        _infoUI.SetActive(false);
    }

    public void RefreshAmmoCountText(GunItem gunItem)
    {
        _ammoCountText.text = gunItem.CurrentAmmoCount.ToString() + "/" + gunItem.MagazineCapacity;
    }
}
