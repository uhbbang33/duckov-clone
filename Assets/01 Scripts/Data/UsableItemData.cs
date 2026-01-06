[System.Serializable]
public class UsableItemData
{
    public uint Id;
    public string ItemType;
    public string Name;
    public uint Value;
    public float Weight;
    public float HealHP;
    public uint DurabilityCost;
    public float Hunger;
    public float Hydration;
    public float WeightValue;
}

[System.Serializable]
public class UsableItemDataList
{
    public UsableItemData[] UsableItemDatas;
}
