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
                // ItemType으로 안받기 때문에 오류남
                // string으로 받은다음 ItemType으로 변환
                // 또는 그냥 string 값으로 - bulletType도 마찬가지
                if (data.itemType == ItemType.Food)
                    _foodDatas.Add(data);
                else if(data.itemType == ItemType.Medicine)
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

    public GunData GetGun(int id)
    {
        foreach (var gun in _gunDataList.GunItemDatas)
            if (gun.id == id)
                return gun;
        return null;
    }

    public AmmoData GetAmmo(int id)
    {
        foreach (var ammo in _ammoDataList.AmmoItemDatas)
            if (ammo.id == id)
                return ammo;
        return null;
    }

    public UsableItemData GetUsableItem(int id)
    {
        foreach (var usableItem in _usableItemDataList.UsableItemDatas)
            if (usableItem.id == id)
                return usableItem;
        return null;
    }

    public EtcItemData GetEtcItem(int id)
    {
        foreach (var etcItem in _etcItemDataList.EtcItemDatas)
            if (etcItem.id == id)
                return etcItem;
        return null;
    }

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

    private GunItem CreateItemBasedOnGunData(GunData data)
    {
        GunItem item = new(data.id, data.name, data.value, data.weight, data.weightValue, data.bulletType, data.damage, data.rps, data.magazineCapacity, data.range, data.reloadTime, data.adsTime);

        return item;
    }

    private AmmoItem CreateItemBasedOnAmmoData(AmmoData data)
    {
        AmmoItem item = new(data.id, data.name, data.value, data.weight, data.weightValue, data.bulletType);

        return item;
    }

    private UsableItem CreateItemBasedOnFoodData(UsableItemData data)
    {
        UsableItem item = new(data.id, data.name, data.value, data.weight, data.weightValue, data.healHP, data.durabilityCost, data.Hunger, data.Hydration, ItemType.Food);

        return item;
    }

    private UsableItem CreateItemBasedOnMedicineData(UsableItemData data)
    {
        UsableItem item = new(data.id, data.name, data.value, data.weight, data.weightValue, data.healHP, data.durabilityCost, data.Hunger, data.Hydration, ItemType.Medicine);

        return item;
    }

    private Item CreateItemBasedOnEtcData(EtcItemData data)
    {
        Item item = new(data.id, data.name, data.value, data.value, data.weightValue);

        return item;
    }

    private GunData GetRandomGunData()
    {
        int random = Random.Range(0, _gunDataList.GunItemDatas.Length);

        return _gunDataList.GunItemDatas[random];
    }

    private AmmoData GetRandomAmmoData()
    {
        int random = Random.Range(0, _ammoDataList.AmmoItemDatas.Length);

        return _ammoDataList.AmmoItemDatas[random];
    }

    private UsableItemData GetRandomFoodData()
    {
        int random = Random.Range(0, _foodDatas.Count);

        return _foodDatas[random];
    }

    private UsableItemData GetRandomMedicineData()
    {
        int random = Random.Range(0, _medicineDatas.Count);

        return _medicineDatas[random];
    }

    private EtcItemData GetRandomEtcData()
    {
        int random = Random.Range(0, _etcItemDataList.EtcItemDatas.Length);

        return _etcItemDataList.EtcItemDatas[random];
    }
}
