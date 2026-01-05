using UnityEngine;

public class UIManager : SingletonMonoBehaviour<UIManager>
{
    [SerializeField] private ItemSlot[] _boxSlots;

    public ItemSlot[] BoxSlots { get { return _boxSlots; } }

    public void ChangeBoxSlotUI(int index, uint id)
    {
        if (id == 0)
        {
            _boxSlots[index].RemoveItem();
        }
        else
        _boxSlots[index].SetItem(id);
    }

}
