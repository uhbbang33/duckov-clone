using System.Collections.Generic;
using UnityEngine;

public class DataManager : SingletonMonoBehaviour<DataManager>
{
    private GunDataList _gunDataList;
    private AmmoDataList _ammoDataList;
    private UsableItemDataList _usableItemDataList;
    private EtcItemDataList _etcItemDataList;

    private List<UsableItemData> _foodDatas = new();
    private List<UsableItemData> _medicineDatas = new();

    #region Property
    public GunDataList GunDatas
    {
        get { return _gunDataList; }
        set { _gunDataList = value; }
    }

    public AmmoDataList AmmoDatas
    {
        get { return _ammoDataList; }
        set { _ammoDataList = value; }
    }

    public UsableItemDataList UsableItemDatas
    {
        get { return _usableItemDataList; }
        set {
            _usableItemDataList = value;

            foreach(var data in value.UsableItemDatas)
            {
                if (data.ItemType == ItemType.Food)
                    _foodDatas.Add(data);
                else if(data.ItemType == ItemType.Medicine)
                    _medicineDatas.Add(data);
            }
        }
    }

    public EtcItemDataList EtcItemDatas
    {
        get { return _etcItemDataList; }
        set { _etcItemDataList = value; }
    }

    #endregion Property


    #region Get 
    public GunData GetGun(int id)
    {
        foreach (var gun in _gunDataList.GunItemDatas)
            if (gun.Id == id)
                return gun;
        return null;
    }

    public AmmoData GetAmmo(int id)
    {
        foreach (var ammo in _ammoDataList.AmmoItemDatas)
            if (ammo.Id == id)
                return ammo;
        return null;
    }

    public UsableItemData GetUsableItem(int id)
    {
        foreach (var usableItem in _usableItemDataList.UsableItemDatas)
            if (usableItem.Id == id)
                return usableItem;
        return null;
    }

    public EtcItemData GetEtcItem(int id)
    {
        foreach (var etcItem in _etcItemDataList.EtcItemDatas)
            if (etcItem.Id == id)
                return etcItem;
        return null;
    }

    #endregion Get


    public Item GetRandomItem(string type)
    {
        if (type == ItemType.Gun)
            return CreateItemBasedOnGunData(GetRandomGunData());
        else if (type == ItemType.Ammo)
            return CreateItemBasedOnAmmoData(GetRandomAmmoData());
        else if (type == ItemType.Food)
            return CreateItemBasedOnFoodData(GetRandomFoodData());
        else if (type == ItemType.Medicine)
            return CreateItemBasedOnMedicineData(GetRandomMedicineData());
        else if (type == ItemType.Etc)
            return CreateItemBasedOnEtcData(GetRandomEtcData());

        return null;
    }

    #region Create Item Based on Data
    private GunItem CreateItemBasedOnGunData(GunData data)
    {
        GunItem item = new(data.Id, data.Rarity, data.Name, data.Value, data.Weight, data.WeightValue, data.BulletType, data.Damage, data.Rps, data.MagazineCapacity, data.Range, data.ReloadTime, data.AdsTime, data.MaxStackSize);

        return item;
    }

    private AmmoItem CreateItemBasedOnAmmoData(AmmoData data)
    {
        AmmoItem item = new(data.Id, data.Rarity, data.Name, data.Value, data.Weight, data.WeightValue, data.MaxStackSize, data.BulletType);

        return item;
    }

    private UsableItem CreateItemBasedOnFoodData(UsableItemData data)
    {
        UsableItem item = new(data.Id, data.Rarity, data.Name, data.Value, data.Weight, data.WeightValue, data.HealHP, data.DurabilityCost, data.Hunger, data.Hydration, data.MaxStackSize, ItemType.Food);

        return item;
    }

    private UsableItem CreateItemBasedOnMedicineData(UsableItemData data)
    {
        UsableItem item = new(data.Id, data.Rarity, data.Name, data.Value, data.Weight, data.WeightValue, data.HealHP, data.DurabilityCost, data.Hunger, data.Hydration, data.MaxStackSize, ItemType.Medicine);

        return item;
    }

    private Item CreateItemBasedOnEtcData(EtcItemData data)
    {
        Item item = new(data.Id, data.Rarity, data.Name, data.Value, data.Weight, data.WeightValue, data.MaxStackSize);

        return item;
    }

    #endregion


    //TODO : 중복 코드, 다른 방법 고민해보기
    #region Get Random Data

    private GunData GetRandomGunData()
    {
        float totalWeightValue = 0;
        foreach (var w in _gunDataList.GunItemDatas)
            totalWeightValue += w.WeightValue;

        float random = Random.Range(0, totalWeightValue);
        float current = 0;

        foreach (var w in _gunDataList.GunItemDatas)
        {
            current += w.WeightValue;
            if (random < current)
                return w;
        }

        return null;
    }

    private AmmoData GetRandomAmmoData()
    {
        float totalWeightValue = 0;
        foreach (var w in _ammoDataList.AmmoItemDatas)
            totalWeightValue += w.WeightValue;

        float random = Random.Range(0, totalWeightValue);
        float current = 0;

        foreach (var w in _ammoDataList.AmmoItemDatas)
        {
            current += w.WeightValue;
            if (random < current)
                return w;
        }

        return null;
    }

    private UsableItemData GetRandomFoodData()
    {
        float totalWeightValue = 0;
        foreach (var w in _foodDatas)
            totalWeightValue += w.WeightValue;

        float random = Random.Range(0, totalWeightValue);
        float current = 0;

        foreach (var w in _foodDatas)
        {
            current += w.WeightValue;
            if (random < current)
                return w;
        }

        return null;
    }

    private UsableItemData GetRandomMedicineData()
    {
        float totalWeightValue = 0;
        foreach (var w in _medicineDatas)
            totalWeightValue += w.WeightValue;

        float random = Random.Range(0, totalWeightValue);
        float current = 0;

        foreach (var w in _medicineDatas)
        {
            current += w.WeightValue;
            if (random < current)
                return w;
        }

        return null;
    }

    private EtcItemData GetRandomEtcData()
    {
        float totalWeightValue = 0;
        foreach (var w in _etcItemDataList.EtcItemDatas)
            totalWeightValue += w.WeightValue;

        float random = Random.Range(0, totalWeightValue);
        float current = 0;

        foreach (var w in _etcItemDataList.EtcItemDatas)
        {
            current += w.WeightValue;
            if (random < current)
                return w;
        }

        return null;
    }

    #endregion
}
