using UnityEngine;
using UnityEngine.EventSystems;

public class MainUsableQuickSlotUI : ItemSlotUI
{
    protected override void Start()
    {
        base.Start();

        _itemSlot = new ItemSlot();
        _itemSlot.UI = this;
        _itemSlot.Type = SlotType.MAINQUICKSLOT;
    }

    //TODO : 상호작용 Slot UI class 따로 빼기
    public override void OnBeginDrag(PointerEventData eventData)
    {
        return;
    }

    public override void OnDrop(PointerEventData eventData)
    {
        return;
    }
}
