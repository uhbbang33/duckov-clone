
public class Item
{
    protected uint _id;
    protected string _name;
    protected uint _value;
    protected float _weight;
    protected uint _weightValue;
    protected uint _maxStackSize;
    protected string _itemType;

    public uint ID { get { return _id; } }
    public string Name { get { return _name; } }
    public uint Value { get { return _value; } }
    public float Weight { get { return _weight; } }
    public uint WeightValue { get { return _weightValue; } }
    public uint MaxStackSize { get { return _maxStackSize; } }
    public string Type { get { return _itemType; } }

    public Item(uint id, string name, uint value, float weight, uint weightValue, uint maxStackSize)
    {
        _id = id;
        _name = name;
        _value = value;
        _weight = weight;
        _weightValue = weightValue;
        _maxStackSize = maxStackSize;
    }
}
