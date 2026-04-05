
using System.Collections.Generic;

[System.Serializable]
public class PlayerInventorySaveData
{
    // Quick Slot
    public int EquipedGunSlotNum;
    public List<int> QuickSlotLinkedInventoryIndex;

    // Inventory
    public List<int> ItemIDList;
    public List<int> GunItemAmmoCountList;
    public List<int> QuantityList;
    public List<float> DurabilityList;

    // Money
    public int Money;
}
