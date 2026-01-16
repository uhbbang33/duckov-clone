
public class ItemSlot 
{
    private Item _currentItem;
    private int _quantity;
    private ItemSlotUI _ui;
    private SlotType _slotType;
    
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

    public SlotType Type
    {
        get { return _slotType; }
        set { _slotType = value; }
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

    public void SplitItem(int amount)
    {
        if (_slotType == SlotType.INVENTORY)
            GameManager.Instance.Inventory.AddItemToEmptySlot(_currentItem, amount);
        else if (_slotType == SlotType.BOX)
        {
            GameManager.Instance.CurrentBox.AddItemToEmptySlot(_currentItem, amount);
        }
    }
}
