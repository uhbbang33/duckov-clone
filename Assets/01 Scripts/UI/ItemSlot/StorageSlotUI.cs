using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class StorageSlotUI : ItemSlotUI
{
    public override void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null
            || _itemSlot == null)
            return;

        ItemSlotUI startUI = eventData.pointerDrag.GetComponent<ItemSlotUI>();
        Item startItem = startUI?.Slot.CurrentItem;

        base.OnDrop(eventData);

        if (startUI.Slot.Type == SlotType.INVENTORY
            && startUI.LinkedQuickSlot != null)
        {
            startUI.LinkedQuickSlot.UnlinkInventorySlotUI(startItem.ID);
        }
    }

    protected override void OnDoubleClick()
    {
        base.OnDoubleClick();

        TryMoveToInventoryByDoubleClick();
    }

}
