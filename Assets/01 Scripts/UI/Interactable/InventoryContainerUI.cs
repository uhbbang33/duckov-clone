
public class InventoryContainerUI : InteractableStateUI
{
    public override void OnInteract()
    {
        base.OnInteract();
        FieldManager.Instance.Inventory.OnInventoryOpenWithInteractable();
    }
}
