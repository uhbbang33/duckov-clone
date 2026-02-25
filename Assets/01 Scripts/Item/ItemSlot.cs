using UnityEngine;

public class ItemSlot 
{
    protected Item _currentItem;
    protected ItemSlotUI _ui;
    private Inventory _inventory;

    protected SlotType _slotType;
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


    public virtual void SubtractItem(int amount = 1)
    {
        if (amount == 0) return;

        _quantity -= amount;

        if (_slotType == SlotType.INVENTORY)
        {
            _inventory.ChangeWeight(false, _currentItem.Weight * amount);
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

    public virtual void AddItem(Item item, ref int amount)
    {
        if (amount == 0 || item == null) return;

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

        amount -= addAmount;
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

    public virtual void UseItem()
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

        if(_ui.LinkedQuickSlot != null && _quantity == 0)
        {
            _ui.LinkedQuickSlot.UnlinkInventorySlotUI(item.ID);
        }
    }

    public virtual void UnloadAmmo()
    {
        GunItem gunItem = _currentItem as GunItem;

        if (gunItem.CurrentAmmoCount <= 0)
            return;

        AmmoItem ammoItem = gunItem.Ammo;

        int ammoCount = gunItem.CurrentAmmoCount;
        if(!_inventory.TryAddItem(ammoItem, ref ammoCount))
        {
            // ¹ö¸®±â
            GameManager.Instance.CreateDropItemObject(ammoItem, ammoCount);
        }

        gunItem.CurrentAmmoCount = 0;
    }
}
