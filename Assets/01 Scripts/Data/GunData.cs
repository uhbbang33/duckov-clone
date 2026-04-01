
[System.Serializable]
public class GunData : ItemData
{
    public string BulletType;
    public float Damage;
    public float Rps;
    public uint MagazineCapacity;
    public float Range;
    public float ReloadTime;
    public float AdsTime;
    public float SoundRange;
    public int EnemyFireCount;
}

[System.Serializable]
public class GunDataList
{
    // 주의 - 무조건 json 파일 최상단의 이름과 같아야 함
    public GunData[] GunItemDatas;
}
