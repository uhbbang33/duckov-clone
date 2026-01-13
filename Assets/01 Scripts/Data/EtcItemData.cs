[System.Serializable]
public class EtcItemData
{
    public uint Id;
    public string Rarity;
    public string ItemType;
    public string Name;
    public uint Value;
    public float Weight;
    public uint WeightValue;
    public uint MaxStackSize;
}

[System.Serializable]
public class EtcItemDataList
{
    public EtcItemData[] EtcItemDatas;
}
