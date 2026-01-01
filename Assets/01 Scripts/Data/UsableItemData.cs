[System.Serializable]
public class UsableItemData
{
    public uint id;
    public ItemType itemType;
    public string name;
    public uint value;
    public float weight;
    public float healHP;
    public uint durabilityCost;
    public float Hunger;
    public float Hydration;
    public float weightValue;
}

[System.Serializable]
public class UsableItemDataList
{
    public UsableItemData[] UsableItemDatas;
}
