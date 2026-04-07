
using System.Collections.Generic;

[System.Serializable]
public class PlayerInventorySaveData
{
    // Quick Slot
    public int EquipedGunSlotNum;
    public List<int> QuickSlotLinkedInventoryIndex;

    // Gun Slot
    public List<int> GunSlotItemIDList;
    public List<int> GunSlotItemAmmoCountList;

    // Inventory
    public List<int> ItemIDList;
    public List<int> GunItemAmmoCountList;
    public List<int> QuantityList;
    public List<int> DurabilityList;

    // Money
    public int Money;
}
