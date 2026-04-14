
public class BoxSlotUI : ItemSlotUI
{
    protected override void OnDoubleClick()
    {
        base.OnDoubleClick();

        TryMoveToInventoryByDoubleClick();
    }
}
