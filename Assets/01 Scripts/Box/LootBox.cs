using System.Collections;
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
        _typeWeights[0].WeightValue = 0;
        _typeWeights[1].WeightValue = 0;
        _typeWeights[2].WeightValue = 5;
        _typeWeights[3].WeightValue = 5;
        _typeWeights[4].WeightValue = 5;
    }

    protected override void SetBoxItems()
    {
        DataManager dataManager = DataManager.Instance;

        // boxSlot 0 - Gun
        GunItem gunItem = dataManager.CreateItemBasedOnGunData(_enemyGunData);
        int quantity = 1;

        _boxSlots[0].AddItem(gunItem, ref quantity);


        // boxSlot 1 - Ammo
        AmmoData ammoData = dataManager.GetAmmo(_enemyGunData.BulletType);
        AmmoItem ammoItem = dataManager.CreateItemBasedOnAmmoData(ammoData);

        if (_enemyGunData.BulletType == BulletType.S)
            quantity = Random.Range(52, 91);
        else if (_enemyGunData.BulletType == BulletType.Sniping)
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
