[System.Serializable]
public class AmmoData : ItemData
{
    public string AmmoType;

    public override Item ToItem()
    {
        return new AmmoItem(Id, Rarity, Name, Value, Weight, WeightValue, MaxStackSize, AmmoType);
    }
}

[System.Serializable]
public class AmmoDataList
{
    public AmmoData[] AmmoItemDatas;
}
