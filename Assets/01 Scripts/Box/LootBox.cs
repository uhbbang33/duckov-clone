using UnityEngine;

public class LootBox : Box
{
    private GunData _enemyGunData;
    public GunData EnemyGunData
    {
        get { return _enemyGunData; }
        set { _enemyGunData = value; }
    }

    protected override void SetWeightValue()
    {
        _typeWeights[ItemType.Medicine] = 5;
        _typeWeights[ItemType.Food] = 5;
        _typeWeights[ItemType.Etc] = 5;
    }

    protected override void ChangeBoxText()
    {
        UIManager.Instance.ChangeBoxItemCountText("전리품", _filledSlotCnt, _slotCnt);
    }

    protected override void SetBoxItems()
    {
        DataManager dataManager = DataManager.Instance;

        // boxSlot 0 - Gun
        GunItem gunItem = _enemyGunData.ToItem() as GunItem;
        int quantity = 1;

        _boxSlots[0].AddItem(gunItem, ref quantity);


        // boxSlot 1 - Ammo
        AmmoData ammoData = dataManager.GetAmmo(_enemyGunData.AmmoType);
        AmmoItem ammoItem = ammoData.ToItem() as AmmoItem;

        if (_enemyGunData.AmmoType == AmmoType.S)
            quantity = Random.Range(52, 91);
        else if (_enemyGunData.AmmoType == AmmoType.Sniping)
            quantity = Random.Range(30, 61);

        _boxSlots[1].AddItem(ammoItem, ref quantity);


        // boxSlot 2 ~ 4 Random
        int itemCnt = Random.Range(3, _slotCnt + 1);

        for (int i = 2; i < itemCnt; ++i)
        {
            Item item = GetRandomItemByType();

            int itemQuantity = 1;
            _boxSlots[i].AddItem(item, ref itemQuantity);
        }

        _filledSlotCnt = itemCnt;
    }
}
