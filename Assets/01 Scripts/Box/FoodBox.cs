public class FoodBox : Box
{
    protected override void SetWeightValue()
    {
        _typeWeights[0].WeightValue = 0;
        _typeWeights[1].WeightValue = 0;
        _typeWeights[2].WeightValue = 2;
        _typeWeights[3].WeightValue = 10;
        _typeWeights[4].WeightValue = 0;
    }
}
