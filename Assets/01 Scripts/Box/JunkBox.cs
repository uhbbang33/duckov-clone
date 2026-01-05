public class JunkBox : Box
{
    protected override void SetWeightValue()
    {
        _typeWeights[0].WeightValue = 1;
        _typeWeights[1].WeightValue = 2;
        _typeWeights[2].WeightValue = 4;
        _typeWeights[3].WeightValue = 4;
        _typeWeights[4].WeightValue = 10;
    }
}
