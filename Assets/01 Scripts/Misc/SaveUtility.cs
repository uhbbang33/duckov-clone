using System.Collections.Generic;
using UnityEngine;

public static class SaveUtility
{
    public static List<int> GetSlotItemsID(ISaveableContainer container)
    {
        List<int> list = new List<int>();
        var slots = container.GetSlots();

        foreach (var slot in slots)
        {
            if (slot.CurrentItem != null)
                list.Add((int)slot.CurrentItem.ID);
            else
                list.Add(-1);
        }

        return list;
    }


    public static List<int> GetSlotGunItemsAmmoCount(ISaveableContainer container)
    {
        List<int> list = new List<int>();
        var slots = container.GetSlots();

        foreach (var slot in slots)
        {
            if (slot.CurrentItem != null
                && slot.CurrentItem.Type == ItemType.Gun)
            {
                int cnt = (slot.CurrentItem as GunItem).CurrentAmmoCount;

                list.Add(cnt);
            }
            else
                list.Add(-1);
        }

        return list;
    }

    public static List<int> GetSlotsQuantity(ISaveableContainer container)
    {
        List<int> list = new List<int>();
        var slots = container.GetSlots();

        foreach (var slot in slots)
        {
            list.Add(slot.Quantity);
        }

        return list;
    }

    public static List<float> GetSlotItemsDurability(ISaveableContainer container)
    {
        List<float> list = new List<float>();
        var slots = container.GetSlots();

        foreach (var slot in slots) { 
            Item item = slot.CurrentItem;
            if (item == null
                || (item.Type != ItemType.Medicine
                && item.Type != ItemType.Food))
            {
                list.Add(-1);
                continue;
            }

            float durability = (item as UsableItem).CurrentDurability;
            durability = Mathf.Floor(durability);

            list.Add(durability);
        }

        return list;
    }

}
