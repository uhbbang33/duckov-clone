using UnityEngine;

public class StarterBox : Box
{
    [SerializeField] private GunType _gunType = GunType.Glock;
    [SerializeField] private int _ammoNum = 30;
    [SerializeField] private int _usableItemId = 10;
    [SerializeField] private int _usableItemQuantity = 3;

    protected override void Start()
    {
        base.Start();
        _allRarityOpened = true;
    }

    protected override void ChangeBoxText()
    {
        UIManager.Instance.ChangeBoxItemCountText("보급 상자", _filledSlotCnt, _slotCnt);
    }

    protected override void SetWeightValue()
    {
    }

    protected override void SetBoxItems()
    {
        DataManager dataManager = DataManager.Instance;

        GunData gunData = dataManager.GetGunByType(_gunType);
        GunItem gunItem = gunData.ToItem() as GunItem;

        int quantity = 1;
        
        // boxSlot 0 - Gun
        _boxSlots[0].AddItem(gunItem, ref quantity);

        // boxslot 1 - Ammo
        AmmoData ammoData = dataManager.GetAmmo(gunData.AmmoType);
        AmmoItem ammoItem = ammoData.ToItem() as AmmoItem;
        quantity = _ammoNum;

        _boxSlots[1].AddItem(ammoItem, ref quantity);

        // boxSlot 2 - HP Item
        UsableItem usableItem = dataManager.GetItemDataByID(_usableItemId).ToItem() as UsableItem;
        quantity = _usableItemQuantity;
        _boxSlots[2].AddItem(usableItem, ref quantity);
    }
}
