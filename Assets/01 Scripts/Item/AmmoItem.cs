
public class AmmoItem : Item
{
    private ItemType _itemType;
    private BulletType _bulletType;

    public ItemType AmmoItemType { get { return _itemType; } }
    public BulletType AmmoBulletType { get { return _bulletType; } }

    public AmmoItem(uint id, string name, uint value, float weight, float weightValue, BulletType bulletType) : base(id, name, value, weight, weightValue)
    {
        _itemType = ItemType.Ammo;
        _bulletType = bulletType;
    }

}
