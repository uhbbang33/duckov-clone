
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
        set
        {
            _currentItem = value;
            _ui.SetInfoUI(_currentItem);
        }
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
        if (amount == 0) return;

        _quantity -= amount;

        if (_slotType == SlotType.INVENTORY)
        {
            GameManager.Instance.Inventory.LoseWeight(_currentItem.Weight * amount);
        }

        if (_quantity <= 0)
        {
            if (_slotType == SlotType.INVENTORY)
            {
                GameManager.Instance.Inventory.RemoveItemSlot(_currentItem.ID);
            }
            else if (_slotType == SlotType.BOX)
            {
                GameManager.Instance.CurrentOpenBox.ChangeBoxItemCount(false);
            }

            _currentItem = null;
            _quantity = 0;
            
        }

        _ui.RefreshUI();
    }

    public int AddItem(Item item, int amount = 1)
    {
        if (amount == 0) return amount;

        if (_currentItem == null)
        {
            if (_slotType == SlotType.INVENTORY)
                GameManager.Instance.Inventory.AddToDictionaryByID(item.ID);

            if (_slotType == SlotType.BOX)
                GameManager.Instance.CurrentOpenBox.ChangeBoxItemCount(true);
        }

        _currentItem = item;
        _ui.SetInfoUI(_currentItem);

        int addableItemCount = (int)item.MaxStackSize - _quantity;
        int addAmount = 0;

        if(addableItemCount >= amount)
        {
            _quantity += amount;
            addAmount = amount;
        }
        else
        {
            _quantity += addableItemCount;
            addAmount = addableItemCount;
        }

        if (_slotType == SlotType.INVENTORY)
        {
            GameManager.Instance.Inventory.AddWeight(_currentItem.Weight * addAmount);
        }

        _ui.RefreshUI();

        return amount - addAmount;
    }

    public void SplitItem(int amount)
    {
        SubtractItem(amount);

        if (_slotType == SlotType.INVENTORY)
        {
            GameManager.Instance.Inventory.AddItemToEmptySlot(_currentItem, amount);
        }
        else if (_slotType == SlotType.BOX)
        {
            GameManager.Instance.CurrentBox.AddItemToEmptySlot(_currentItem, amount);
        }
    }
}
