[System.Serializable]
public class EtcItemData
{
    public uint id;
    public string itemType;
    public string name;
    public uint value;
    public float weight;
    public float weightValue;
}

[System.Serializable]
public class EtcItemDataList
{
    public EtcItemData[] EtcItemDatas;
}
