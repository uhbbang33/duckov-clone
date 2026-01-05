public class WeaponBox : Box
{
    protected override void SetWeightValue()
    {
        // TODO : 하드코딩
        _typeWeights[0].WeightValue = 5;
        _typeWeights[1].WeightValue = 10;
        _typeWeights[2].WeightValue = 1;
        _typeWeights[3].WeightValue = 0;
        _typeWeights[4].WeightValue = 0;
    }
}
