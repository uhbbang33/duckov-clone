[System.Serializable]
public class AmmoData
{
    public uint id;
    public ItemType itemType;
    public BulletType bulletType;
    public string name;
    public uint value;
    public float weight;
    public float weightValue;
}

[System.Serializable]
public class AmmoDataList
{
    public AmmoData[] AmmoItemDatas;
}
