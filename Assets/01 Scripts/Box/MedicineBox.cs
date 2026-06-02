
public class MedicineBox : Box
{
    protected override void SetWeightValue()
    {
        _typeWeights[ItemType.Medicine] = 10;
        _typeWeights[ItemType.Food] = 2;
    }

    protected override void ChangeBoxText()
    {
        UIManager.Instance.ChangeBoxItemCountText("약품 상자", _filledSlotCnt, _slotCnt);
    }
}
