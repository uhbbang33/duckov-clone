public class WeaponBox : Box
{
    protected override void SetWeightValue()
    {
        _typeWeights[ItemType.Gun] = 5;
        _typeWeights[ItemType.Ammo] = 10;
        _typeWeights[ItemType.Medicine] = 1;
    }

    protected override void ChangeBoxText()
    {
        UIManager.Instance.ChangeBoxItemCountText("무기 상자", _filledSlotCnt, _slotCnt);
    }
}
