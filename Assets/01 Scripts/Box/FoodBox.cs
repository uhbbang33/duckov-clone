public class FoodBox : Box
{
    protected override void SetWeightValue()
    {
        _typeWeights[ItemType.Medicine] = 2;
        _typeWeights[ItemType.Food] = 10;
    }

    protected override void ChangeBoxText()
    {
        UIManager.Instance.ChangeBoxItemCountText("음식 상자", _filledSlotCnt, _slotCnt);
    }
}
