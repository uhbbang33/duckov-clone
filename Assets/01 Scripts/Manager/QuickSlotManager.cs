using System.Collections.Generic;
using UnityEngine;

public class QuickSlotManager : SingletonMonoBehaviour<QuickSlotManager>
{
    // key - id, value - location(3~8)
    private Dictionary<int, int> _quickSlotDictionary;
    [SerializeField] private QuickSlot[] _equipQuickSlots;

    private const int _quickSlotStartNum = 3;

    protected override void Awake()
    {
        base.Awake();

        _quickSlotDictionary = new Dictionary<int, int>();
    }

    public void AddDict(int id, int location)
    {
        int sameItemLocation = FindSameItemLocation(id);

        // 같은 아이템이 다른 퀵슬롯에 있을경우 
        if (sameItemLocation != -1)
        {
            if (sameItemLocation == location)
                return;

            //해당 퀵슬롯 등록 해제
            _equipQuickSlots[sameItemLocation - _quickSlotStartNum].UnlinkInventorySlotUI((uint)id);
        }

        _quickSlotDictionary.Add(id, location);
    }

    private int FindSameItemLocation(int itemId)
    {
        if(!_quickSlotDictionary.TryGetValue(itemId, out int value))
            return -1;

        return value;
    }

    public void RemoveDict(int itemId)
    {
        _quickSlotDictionary.Remove(itemId);
    }
}
