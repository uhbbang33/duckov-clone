using UnityEngine;

public class Item
{
    protected uint _id;
    protected string _name;
    protected uint _value;
    protected float _weight;
    protected float _weightValue;

    public uint ID { get { return _id; } }
    public string Name { get { return _name; } }
    public uint Value { get { return _value; } }
    public float Weight { get { return _weight; } }
    public float WeightValue { get { return _weightValue; } }

    public Item(uint id, string name, uint value, float weight, float weightValue)
    {
        _id = id;
        _name = name;
        _value = value;
        _weight = weight;
        _weightValue = weightValue;
    }
}
