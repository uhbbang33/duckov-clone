[System.Serializable]
public class UsableItemData : ItemData
{
    public float HealHP;
    public uint DurabilityCost;
    public float Hunger;
    public float Hydration;
}

[System.Serializable]
public class UsableItemDataList
{
    public UsableItemData[] UsableItemDatas;
}
