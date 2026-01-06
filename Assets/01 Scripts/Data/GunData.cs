
[System.Serializable]
public class GunData
{
    public uint Id;
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
    public float WeightValue;
}

[System.Serializable]
public class GunDataList
{
    // 주의 - 무조건 json 파일 최상단의 이름과 같아야 함
    public GunData[] GunItemDatas;
}
