
[System.Serializable]
public class GunData
{
    public uint Id;
    public string Rarity;
    public string ItemType;
    public string Name;
    public string BulletType;
    public uint Value;
    public float Weight;
    public float Damage;
    public float Rps;
    public uint MagazineCapacity;
    public float Range;
    public float ReloadTime;
    public float AdsTime;
    public uint WeightValue;
    public uint MaxStackSize;

    //public GunData(GunData data)
    //{
    //    Id = data.Id;
    //    Rarity = data.Rarity;
    //    ItemType = data.ItemType;
    //    Name = data.Name;
    //    BulletType = data.BulletType;
    //    Value = data.Value;
    //    Weight = data.Weight;
    //    Damage = data.Damage;
    //    Rps = data.Rps;
    //    MagazineCapacity = data.MagazineCapacity;
    //    Range = data.Range;
    //    ReloadTime = data.ReloadTime;
    //    AdsTime = data.AdsTime;
    //    WeightValue = data.WeightValue;
    //    MaxStackSize = data.MaxStackSize;
    //}
}

[System.Serializable]
public class GunDataList
{
    // 주의 - 무조건 json 파일 최상단의 이름과 같아야 함
    public GunData[] GunItemDatas;
}
