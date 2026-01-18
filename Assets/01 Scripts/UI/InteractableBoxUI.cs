
public class InteractableBoxUI : InteractableStateUI
{
    private bool _isOpened;

    private void Awake()
    {
        _isOpened = false;
    }

    public override void Selected()
    {
        base.Selected();

        _stateImage.sprite = GameResources.Instance.OpenableBoxSprite;
    }

    public override void Deselected()
    {
        base.Deselected();

        if (_isOpened)
            _stateImage.sprite = GameResources.Instance.OpenedBoxSprite;
        else
            _stateImage.sprite = GameResources.Instance.UnopenedBoxSprite;
    }

    public override void OnInteract()
    {
        base.OnInteract();

        GameManager.Instance.Inventory.OnInventoryOpenWithBox();
    }
}
