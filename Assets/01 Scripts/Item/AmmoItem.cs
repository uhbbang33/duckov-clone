
public class AmmoItem : Item
{
    //private string _itemType;
    private string _bulletType;

    public string AmmoItemType { get { return _itemType; } }
    public string AmmoBulletType { get { return _bulletType; } }

    public AmmoItem(uint id, string name, uint value, float weight, float weightValue, string bulletType) : base(id, name, value, weight, weightValue)
    {
        _itemType = ItemType.Ammo;
        _bulletType = bulletType;
    }

}
