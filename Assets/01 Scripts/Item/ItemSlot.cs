public class ItemSlot
{
    private Item _currentItem;
    
    public Item CurrentItem
    {
        get { return _currentItem; }
        set { _currentItem = value; }
    }


    public void RemoveItem()
    {
        _currentItem = null;
    }
}
