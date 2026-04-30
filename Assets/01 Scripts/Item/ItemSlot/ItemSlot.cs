using UnityEngine;

public class ItemSlot 
{
    protected Item _currentItem;
    protected ItemSlotUI _ui;
    private GameManager _gameManager;
    private Inventory _inventory;

    protected SlotType _slotType;
    protected int _quantity;
    private int _inventoryIndex = -1;

    public ItemSlot()
    {
        _currentItem = null;
        _quantity = 0;
        _ui = null;
        _gameManager = GameManager.Instance;
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

    public void InitInventory(Inventory inventory)
    {
        _inventory = inventory;
    }

    public virtual void SubtractItem(int amount = 1)
    {
        if (amount == 0) return;

        _quantity -= amount;

        if (_slotType == SlotType.INVENTORY)
        {
            _inventory.ChangeWeight(false, _currentItem.Weight * amount);

            if (_currentItem.Type == ItemType.Ammo)
                _inventory.ReduceAmmoCount(_currentItem.ID, amount);
        }

        if (_quantity <= 0)
        {
            if (_slotType == SlotType.INVENTORY)
                _inventory.RemoveItemSlot(_currentItem.ID);
            else if (_slotType == SlotType.BOX)
                FieldManager.Instance.CurrentOpenBox.ChangeBoxItemCount(false);
            else if (_slotType == SlotType.STORAGE)
                BunkerManager.Instance.storage.ChangeStorageItemCount(false);

            _currentItem = null;
            _quantity = 0;
            
        }

        _ui.RefreshUI();
    }

    public virtual void AddItem(Item item, ref int amount)
    {
        if (amount == 0 || item == null) return;

        int addableItemCount = (int)item.MaxStackSize - _quantity;
        if (addableItemCount <= 0)
            return;

        int addAmount = 0;

        if (_currentItem == null)
        {
            if (_slotType == SlotType.INVENTORY)
                _inventory.AddToInventoryDictByID(item.ID);
            else if (_slotType == SlotType.BOX)
                FieldManager.Instance.CurrentOpenBox.ChangeBoxItemCount(true);
            else if (_slotType == SlotType.STORAGE)
                BunkerManager.Instance.storage.ChangeStorageItemCount(true);
        }

        _currentItem = item;

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

            if (item.Type == ItemType.Ammo)
                _inventory.AddToAmmoDictByID(item.ID, addAmount);
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
            FieldManager.Instance.CurrentBox.AddItemToEmptySlot(_currentItem, amount);
        }
        else if(_slotType == SlotType.STORAGE)
        {
            BunkerManager.Instance.storage.AddItemToEmptySlot(_currentItem, amount);
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
                _ui.RefreshUI();
                return;
            }
        }
        
        SubtractItem();

        if(_ui.LinkedQuickSlot != null && _quantity == 0)
        {
            _ui.LinkedQuickSlot.UnlinkInventorySlotUI(item.ID);
        }
    }

    public void DiscardItem()
    {
        if (_gameManager.CreateDropItemObject(_currentItem, _quantity))
        {
            InventorySlotUI inventorySlotUI = (_ui as InventorySlotUI);

            if (inventorySlotUI != null
                && inventorySlotUI.LinkedQuickSlot != null)
                inventorySlotUI.LinkedQuickSlot.UnlinkInventorySlotUI(_currentItem.ID);

            SubtractItem(_quantity);
            SoundManager.Instance.PlaySFXOneShot(SFXName.PickItem);
        }
        else
        {
            Debug.Log("버릴 수 없습니다.");
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
            // 버리기
            _gameManager.CreateDropItemObject(ammoItem, ammoCount);
        }

        gunItem.CurrentAmmoCount = 0;
        _ui.RefreshUI();
    }

}
