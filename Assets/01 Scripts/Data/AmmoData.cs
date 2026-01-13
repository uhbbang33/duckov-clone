[System.Serializable]
public class AmmoData
{
    public uint Id;
    public string Rarity;
    public string ItemType;
    public string BulletType;
    public string Name;
    public uint Value;
    public float Weight;
    public uint WeightValue;
    public uint MaxStackSize;
}

[System.Serializable]
public class AmmoDataList
{
    public AmmoData[] AmmoItemDatas;
}
