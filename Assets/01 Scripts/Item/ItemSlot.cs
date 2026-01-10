
public class ItemSlot 
{
    private Item _currentItem;
    private int _quantity;
    private ItemSlotUI _ui;

    public ItemSlot()
    {
        _currentItem = null;
        _quantity = 0;
        _ui = null;
    }

    public Item CurrentItem
    {
        get { return _currentItem; }
        set { _currentItem = value; }
    }

    public int Quantity
    {
        get { return _quantity; }
        set { _quantity = value; }
    }

    public ItemSlotUI UI
    {
        get { return _ui; }
        set
        {
            _ui = value;
            _ui.Slot = this;
        }
    }

    public void SubtractItem(int amount = 1)
    {
        _quantity -= amount;
        if (_quantity <= 0)
        {
            _currentItem = null;
            _quantity = 0;
        }
    }

    public void AddItem(Item item, int amount = 1)
    {
        _currentItem = item;
        _quantity += amount;
    }
}
