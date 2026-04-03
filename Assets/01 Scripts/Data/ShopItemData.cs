[System.Serializable]
public class ShopItemData
{
    public uint SlotNum;
    public uint Id;
    public string Name;
}


[System.Serializable]
public class ShopItemDataList
{
    public ShopItemData[] ShopItemDatas;
}
