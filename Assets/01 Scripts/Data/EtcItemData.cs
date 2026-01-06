[System.Serializable]
public class EtcItemData
{
    public uint Id;
    public string ItemType;
    public string Name;
    public uint Value;
    public float Weight;
    public float WeightValue;
}

[System.Serializable]
public class EtcItemDataList
{
    public EtcItemData[] EtcItemDatas;
}
