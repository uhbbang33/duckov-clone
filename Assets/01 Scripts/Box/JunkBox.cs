public class JunkBox : Box
{
    protected override void SetWeightValue()
    {
        _typeWeights[ItemType.Gun] = 1;
        _typeWeights[ItemType.Ammo] = 2;
        _typeWeights[ItemType.Medicine] = 4;
        _typeWeights[ItemType.Food] = 4;
        _typeWeights[ItemType.Etc] = 10;
    }
}
