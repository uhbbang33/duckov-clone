
[System.Serializable]
public class GunData
{
    public uint id;
    public string itemType;
    public string name;
    public string bulletType;
    public uint value;
    public float weight;
    public float damage;
    public float rps;
    public uint magazineCapacity;
    public float range;
    public float reloadTime;
    public float adsTime;
    public float weightValue;
}

[System.Serializable]
public class GunDataList
{
    // 주의 - 무조건 json 파일 최상단의 이름과 같아야 함
    public GunData[] GunItemDatas;
}
