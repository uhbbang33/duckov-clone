using System.Collections.Generic;

public interface ISaveableContainer
{
    IEnumerable<ItemSlot> GetSlots();
}
