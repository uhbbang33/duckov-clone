[System.Serializable]
public class AmmoData : ItemData
{
    public string BulletType;
}

[System.Serializable]
public class AmmoDataList
{
    public AmmoData[] AmmoItemDatas;
}
