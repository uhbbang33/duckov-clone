
public class UsableItem : Item
{
    private float _healHP;
    private uint _durabilityCost;
    private float _hunger;
    private float _hydration;

    public string UsableItemType { get { return _itemType; } }
    public float HealHP { get { return _healHP; } } 
    public uint DurabilityCost { get { return _durabilityCost; } }
    public float Hunger {  get { return _hunger; } }
    public float Hydration { get { return _hydration; } }

    public UsableItem(uint id, string name, uint value, float weight, uint weightValue, float healHP, uint durabilityCost, float hunger, float hydration, uint maxStackSize, string itemType) : base(id, name, value, weight, weightValue, maxStackSize)
    {
        _healHP = healHP;
        _durabilityCost = durabilityCost;
        _hunger = hunger;
        _hydration = hydration;
        _itemType = itemType;
    }

}
