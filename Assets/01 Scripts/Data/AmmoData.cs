[System.Serializable]
public class AmmoData
{
    public uint id;
    public string itemType;
    public string bulletType;
    public string name;
    public uint value;
    public float weight;
    public float weightValue;
}

[System.Serializable]
public class AmmoDataList
{
    public AmmoData[] AmmoItemDatas;
}
