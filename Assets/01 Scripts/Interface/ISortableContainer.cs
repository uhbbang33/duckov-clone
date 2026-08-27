
using System.Collections.Generic;

public interface ISortableContainer
{
    List<ItemSlot> GetSortableSlots();
    void OnSortCompleted() { }
}
