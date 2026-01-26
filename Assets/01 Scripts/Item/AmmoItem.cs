
public class AmmoItem : Item
{
    private string _bulletType;

    public string AmmoItemType { get { return _itemType; } }
    public string AmmoBulletType { get { return _bulletType; } }

    public AmmoItem(uint id, string rarity, string name, uint value, float weight, uint weightValue, uint maxStackSize, string bulletType) : base(id, rarity, name, value, weight, weightValue, maxStackSize)
    {
        _itemType = ItemType.Ammo;
        _bulletType = bulletType;
    }

}
