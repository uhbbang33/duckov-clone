using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlotUI : ItemSlotUI
{
    private QuickSlot _linkedQuickSlot;

    // TODO : setter에 _linkedQuickSlot = value;만 있도록 수정
    public override QuickSlot LinkedQuickSlot
    {
        get { return _linkedQuickSlot; }
        set
        {
            if (_linkedQuickSlot != null)
                _linkedQuickSlot.LinkedInventorySlotUI = null;

            _linkedQuickSlot = value;

            if (value != null)
                value.LinkedInventorySlotUI = this;
        }
    }

    public override void OnDrop(PointerEventData eventData)
    {

        if (eventData.pointerDrag == null
            || _itemSlot == null)
            return;

        ItemSlotUI startUI = eventData.pointerDrag.GetComponent<ItemSlotUI>();

        if (startUI == null
            || !CheckTypeBeforeDrop(startUI.Slot))
            return;

        if (startUI == this)
        {
            if (_infoUI != null)
                _infoUI.ShowUI();
            return;
        }


        Item startItem = startUI.Slot.CurrentItem;
        Item endItem = _itemSlot.CurrentItem;
        if (startItem != null
            && endItem != null
            && startItem.ID == endItem.ID)
        {
            CombineItem(startUI);
        }
        else if (startItem != null)
        {
            SwapQuickSlot(startUI, _linkedQuickSlot);
            SwapItem(startUI);
        }

        QuickSlot startQuickSlot = eventData.pointerDrag?.GetComponent<QuickSlot>();

        if (startQuickSlot == null || startQuickSlot == _linkedQuickSlot)
            return;

        InventorySlotUI startInventorySlot = startQuickSlot.LinkedInventorySlotUI;
        if (startInventorySlot == null)
            return;

        QuickSlot currentQuickSlot = _linkedQuickSlot;

        SwapItem(startInventorySlot);
        startQuickSlot.LinkToInventorySlotUI(this);

        if (currentQuickSlot != null)
            currentQuickSlot.LinkToInventorySlotUI(startInventorySlot);
    }

    protected override void CombineItem(ItemSlotUI startUI)
    {
        int remainItemCount = startUI.Slot.Quantity;
        Item startItem = startUI.Slot.CurrentItem;

        _itemSlot.AddItem(startItem, ref remainItemCount);

        InventorySlotUI inventoryStartUI = startUI as InventorySlotUI;
        if (remainItemCount == startUI.Slot.Quantity)
        {
            SwapQuickSlot(startUI, inventoryStartUI?._linkedQuickSlot);
            SwapItem(startUI);
        }
        else
        {
            int subtractCount = startUI.Slot.Quantity - remainItemCount;
            startUI.Slot.SubtractItem(subtractCount);

            if (startUI.Slot.CurrentItem == null && inventoryStartUI?._linkedQuickSlot != null)
                inventoryStartUI._linkedQuickSlot.UnlinkInventorySlotUI(startItem.ID);
        }
    }

    private void SwapQuickSlot(ItemSlotUI startUI, QuickSlot currentQuick)
    {
        InventorySlotUI startInventoryUI = startUI as InventorySlotUI;

        QuickSlot startQuick = startInventoryUI?._linkedQuickSlot;

        if (startQuick != null)
            startQuick.LinkToInventorySlotUI(this);

        if (currentQuick != null)
            currentQuick.LinkToInventorySlotUI(startInventoryUI);
    }


    #region Double Click

    protected override void OnDoubleClick()
    {
        if (_itemSlot.CurrentItem == null)
            return;

        if (_bunkerManager != null)
        {
            if (_bunkerManager.IsStorageOpened)
                TryMoveToContainerByDoubleClick(SlotType.STORAGE);
            else if (_bunkerManager.IsShopOpened)
                SellItem();
        }
        else if(_fieldManager != null)
        {
            if(_fieldManager.CurrentOpenBox != null)
                TryMoveToContainerByDoubleClick(SlotType.BOX);
        }
    }

    private void TryMoveToContainerByDoubleClick(SlotType openContainerType)
    {
        List<ItemSlot> targetSlots = GetContainerSlots(openContainerType);

        // 같은 ID 아이템 먼저
        foreach (ItemSlot targetSlot in targetSlots)
        {
            if (targetSlot.CurrentItem == null ||
                targetSlot.CurrentItem.ID != _itemSlot.CurrentItem.ID)
                continue;

            int remainAmount = _itemSlot.Quantity;
            targetSlot.AddItem(_itemSlot.CurrentItem, ref remainAmount);
            _itemSlot.SubtractItem(_itemSlot.Quantity - remainAmount);

            if (remainAmount == 0)
            {
                _linkedQuickSlot?.UnlinkInventorySlotUI(targetSlot.CurrentItem.ID);
                return;
            }
        }

        // 빈 슬롯에 이동
        foreach (ItemSlot targetSlot in targetSlots)
        {
            if (targetSlot.CurrentItem != null)
                continue;

            int remainAmount = _itemSlot.Quantity;
            targetSlot.AddItem(_itemSlot.CurrentItem, ref remainAmount);
            _itemSlot.SubtractItem(_itemSlot.Quantity - remainAmount);
            _linkedQuickSlot?.UnlinkInventorySlotUI(targetSlot.CurrentItem.ID);
            return;
        }
    }

    private void SellItem()
    {
        _inventory.ChangeMoney((int)_itemSlot.CurrentItem.Value, _itemSlot.Quantity, true);
        _linkedQuickSlot?.UnlinkInventorySlotUI(_itemSlot.CurrentItem.ID);
        _itemSlot.SubtractItem(_itemSlot.Quantity);
    }

    #endregion Double Click

    public override void RefreshUI()
    {
        base.RefreshUI();
        _linkedQuickSlot?.RefreshUI();
    }
}
