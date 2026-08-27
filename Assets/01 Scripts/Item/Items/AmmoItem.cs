
public class AmmoItem : Item
{
    private string _ammoType;

    public string AmmoItemType { get { return _itemType; } }
    public string AmmoType { get { return _ammoType; } }

    public AmmoItem(uint id, string rarity, string name, uint value, float weight, uint weightValue, uint maxStackSize, string ammoType) : base(id, rarity, name, value, weight, weightValue, maxStackSize)
    {
        _itemType = ItemType.Ammo;
        _ammoType = ammoType;
    }

}
