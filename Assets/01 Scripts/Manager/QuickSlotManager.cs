using System.Collections.Generic;
using UnityEngine;

public class QuickSlotManager : SingletonMonoBehaviour<QuickSlotManager>
{
    // key - id, value - location(3~8)
    private Dictionary<int, int> _quickSlotDictionary;
    [SerializeField] private QuickSlot[] _equipQuickSlots;

    protected override void Awake()
    {
        base.Awake();

        _quickSlotDictionary = new Dictionary<int, int>();
    }

    public void AddToQuickSlot(int id, int location)
    {
        int sameItemLocation = FindSameItemLocation(id);

        // 같은 아이템이 다른 퀵슬롯에 있을경우 
        if (sameItemLocation != -1)
        {
            //해당 퀵슬롯 등록 해제
            RemoveQuickSlot(sameItemLocation);
            // Dictionary value 변경
            _quickSlotDictionary[id] = location;
        }
        else // 없을경우
        {
            // Dictionary에 id등록
            _quickSlotDictionary.Add(id, location);
        }
    }

    private int FindSameItemLocation(int itemId)
    {
        if(!_quickSlotDictionary.TryGetValue(itemId, out int value))
            return -1;

        return value;
    }

    private void RemoveQuickSlot(int location)
    {
        _equipQuickSlots[location - 3].UnLinkInventorySlotUI();
    }
}
