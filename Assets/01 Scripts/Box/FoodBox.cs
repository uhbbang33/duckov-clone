public class FoodBox : Box
{
    protected override void SetWeightValue()
    {
        _typeWeights[ItemType.Medicine] = 2;
        _typeWeights[ItemType.Food] = 10;
    }
}
