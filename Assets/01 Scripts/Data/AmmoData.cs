[System.Serializable]
public class AmmoData : ItemData
{
    public string BulletType;

    public override Item ToItem()
    {
        return new AmmoItem(Id, Rarity, Name, Value, Weight, WeightValue, MaxStackSize, BulletType);
    }
}

[System.Serializable]
public class AmmoDataList
{
    public AmmoData[] AmmoItemDatas;
}
