
public class ItemSlot 
{
    protected Item _currentItem;
    protected ItemSlotUI _ui;
    private Inventory _inventory;

    private SlotType _slotType;
    protected int _quantity;
    private int _inventoryIndex = -1;

    public ItemSlot()
    {
        _currentItem = null;
        _quantity = 0;
        _ui = null;
        _inventory = GameManager.Instance.Inventory;
    }

    public Item CurrentItem
    {
        get { return _currentItem; }
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


    public int InventoryIndex
    {
        get { return _inventoryIndex; }
        set { _inventoryIndex = value; }
    }


    public void SubtractItem(int amount = 1)
    {
        if (amount == 0) return;

        _quantity -= amount;

        if (_slotType == SlotType.INVENTORY)
        {
            _inventory.ChangeWeight(false, _currentItem.Weight * amount);

            if (_quantity == 0)
                _ui.RemoveQuickSlotLink();    
        }

        if (_quantity <= 0)
        {
            if (_slotType == SlotType.INVENTORY)
                _inventory.RemoveItemSlot(_currentItem.ID);
            else if (_slotType == SlotType.BOX)
                GameManager.Instance.CurrentOpenBox.ChangeBoxItemCount(false);

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
                _inventory.AddToDictionaryByID(item.ID);

            if (_slotType == SlotType.BOX)
                GameManager.Instance.CurrentOpenBox.ChangeBoxItemCount(true);
        }

        _currentItem = item;

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
            _inventory.ChangeWeight(true, _currentItem.Weight * addAmount);
        }

        _ui.RefreshUI();

        return amount - addAmount;
    }

    public void SplitItem(int amount)
    {
        SubtractItem(amount);

        if (_slotType == SlotType.INVENTORY)
        {
            _inventory.TryAddItemToEmptySlot(_currentItem, amount);
        }
        else if (_slotType == SlotType.BOX)
        {
            GameManager.Instance.CurrentBox.AddItemToEmptySlot(_currentItem, amount);
        }
    }

    public void UseItem()
    {
        UsableItem item = _currentItem as UsableItem;

        if (!GameManager.Instance.PlayerObject.GetComponent<Player>().UseItem(item))
            return;

        if (item.DurabilityCost > 0)
        {
            item.CurrentDurability -= (int)item.DurabilityCost;
            if (item.CurrentDurability > 0)
            {
                _ui.ChangeDurabilitySliderValue(item.CurrentDurability, Durability.MaxDurability);
                return;
            }
        }
        
        SubtractItem();
    }
}
