[System.Serializable]
public class UsableItemData : ItemData
{
    public float HealHP;
    public uint DurabilityCost;
    public float Hunger;
    public float Hydration;

    public override Item ToItem()
    {
        return new UsableItem(Id, Rarity, Name, Value, Weight, WeightValue, HealHP, DurabilityCost, Hunger, Hydration, MaxStackSize, ItemType);
    }
}

[System.Serializable]
public class UsableItemDataList
{
    public UsableItemData[] UsableItemDatas;
}
