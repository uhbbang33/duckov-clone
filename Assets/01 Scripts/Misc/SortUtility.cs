using System;
using System.Collections.Generic;
using System.Linq;

public static class SortUtility
{
    private static readonly Dictionary<string, int> _itemTypePriority = new()
    {
        {ItemType.Gun, 0 },
        {ItemType.Ammo, 1 },
        {ItemType.Medicine, 2 },
        {ItemType.Food, 3 },
        {ItemType.Etc, 4 },
    };

    public static void Sort(ISortableContainer target)
    {
        List<ItemSlot> slots = target.GetSortableSlots();

        var originItems = slots
            .Where(slot => slot.CurrentItem != null) // 빈 슬롯 제외
            .Select(slot => (slot.CurrentItem, slot.Quantity, slot.UI.LinkedQuickSlot))
            .ToList();

        var mergedItems = MergeItem(originItems);

        var sortedItems = mergedItems
            .OrderBy(slot => _itemTypePriority.GetValueOrDefault(slot.item.Type, int.MaxValue)) // Item Type 오름차순으로
            .ThenByDescending(slot => slot.item.Weight) // 무게 내림차순
            .ToList();

        // 초기화
        foreach (ItemSlot slot in slots)
        {
            slot.UI.LinkedQuickSlot = null;
            slot.SubtractItem(slot.Quantity);
        }

        for (int i = 0; i < sortedItems.Count && i <slots.Count; ++i)
        {
            int quantity = sortedItems[i].qauntity;

            slots[i].AddItem(sortedItems[i].item, ref quantity);
            slots[i].UI.LinkedQuickSlot = sortedItems[i].likedQuickSlot;
        }

        target.OnSortCompleted();
    }

    private static List<(Item item, int qauntity, QuickSlot likedQuickSlot)> MergeItem
        (IEnumerable<(Item currentItem, int quantity, QuickSlot linkedQuickSlot)> originItems)
    {
        var groupedById = originItems
            .GroupBy(i => i.currentItem.ID)
            .Select(s => (
            s.First().currentItem,
            s.Sum(i => i.quantity),
            s.First().linkedQuickSlot
            ));

        var result = new List<(Item, int, QuickSlot)>();

        foreach (var (item, totalQuantity, linkedQuickSlot) in groupedById)
        {
            int maxStackSize = (int)item.MaxStackSize;
            int remainQuantity = totalQuantity;
            bool isFirst = true;

            while(remainQuantity > 0)
            {
                int stackSize = Math.Min(remainQuantity, maxStackSize);
                result.Add((item, stackSize, isFirst ? linkedQuickSlot : null));
                remainQuantity -= stackSize;
                isFirst = false;
            }
        }

        return result;
    }
}