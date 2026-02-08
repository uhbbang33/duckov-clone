using UnityEngine.InputSystem;

public class QuickItemSlot : ItemSlot
{
    private int _currentItemInventoryIndex;
    private InputActions _inputActions;

    public QuickItemSlot(int num) : base()
    {
        _currentItemInventoryIndex = -1;
        _inputActions = GameManager.Instance.PlayerObject.GetComponent<Player>().Actions;

        SubscribeInputEvent(num);
    }

    // 게임 종료까지 유지
    private void SubscribeInputEvent(int num)
    {
        switch (num)
        {
            case 3:
                _inputActions.Player.QuickSlot3.performed += UseQuickSlotItem;
                break;
            case 4:
                _inputActions.Player.QuickSlot4.performed += UseQuickSlotItem;
                break;
            case 5:
                _inputActions.Player.QuickSlot5.performed += UseQuickSlotItem;
                break;
            case 6:
                _inputActions.Player.QuickSlot6.performed += UseQuickSlotItem;
                break;
            case 7:
                _inputActions.Player.QuickSlot7.performed += UseQuickSlotItem;
                break;
            case 8:
                _inputActions.Player.QuickSlot8.performed += UseQuickSlotItem;
                break;
        }
    }

    private void UseQuickSlotItem(InputAction.CallbackContext context)
    {
        if (_currentItemInventoryIndex == -1)
            return;

        GameManager.Instance.Inventory.UseInventoryItem(_currentItemInventoryIndex);
    }

    public void ChangeQuickSlot(ItemSlot originItemSlot)
    {
        _currentItem = originItemSlot.CurrentItem;
        _quantity = originItemSlot.Quantity;
        _currentItemInventoryIndex = originItemSlot.InventoryIndex;
        _ui.RefreshUI();
    }

}
