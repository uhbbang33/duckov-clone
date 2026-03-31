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

        var items = slots
            .Where(slot => slot.CurrentItem != null) // 빈 슬롯 제외
            .OrderBy(slot => _itemTypePriority.GetValueOrDefault(slot.CurrentItem.Type, int.MaxValue)) // Item Type 오름차순으로
            .ThenByDescending(slot => slot.CurrentItem.Weight) // 무게 내림차순
            .Select(slot => (slot.CurrentItem, slot.Quantity, slot.UI.LinkedQuickSlot))
            .ToList();

        foreach (ItemSlot slot in slots)
        {
            slot.UI.LinkedQuickSlot = null;
            slot.SubtractItem(slot.Quantity);
        }

        for (int i = 0; i < items.Count; ++i)
        {
            int quantity = items[i].Quantity;

            slots[i].AddItem(items[i].CurrentItem, ref quantity);

            slots[i].UI.LinkedQuickSlot = items[i].LinkedQuickSlot;
        }

        target.OnSortCompleted();
    }
}