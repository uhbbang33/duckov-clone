public class WeaponBox : Box
{
    protected override void SetWeightValue()
    {
        _typeWeights[ItemType.Gun] = 5;
        _typeWeights[ItemType.Ammo] = 10;
        _typeWeights[ItemType.Medicine] = 1;
    }
}
