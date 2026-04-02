
public class InventoryContainerUI : InteractableStateUI
{
    public override void OnInteract()
    {
        base.OnInteract();
        GameManager.Instance.Inventory.OnInventoryOpenWithInteractable();
    }
}
