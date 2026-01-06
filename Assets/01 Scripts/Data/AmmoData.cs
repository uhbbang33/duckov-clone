[System.Serializable]
public class AmmoData
{
    public uint Id;
    public string ItemType;
    public string BulletType;
    public string Name;
    public uint Value;
    public float Weight;
    public float WeightValue;
}

[System.Serializable]
public class AmmoDataList
{
    public AmmoData[] AmmoItemDatas;
}
