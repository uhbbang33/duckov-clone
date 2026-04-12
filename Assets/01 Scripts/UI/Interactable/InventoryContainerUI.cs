
public class InventoryContainerUI : InteractableStateUI
{
    public override void OnInteract()
    {
        base.OnInteract();

        GameManager.Instance.PlayerObject.GetComponent<InventoryController>().OnInventoryOpenWithInteractable();
    }
}
