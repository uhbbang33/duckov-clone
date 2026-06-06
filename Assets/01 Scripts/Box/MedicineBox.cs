
public class MedicineBox : Box
{
    protected override void SetWeightValue()
    {
        _typeWeights[ItemType.Medicine] = 10;
        _typeWeights[ItemType.Food] = 2;
    }
}
