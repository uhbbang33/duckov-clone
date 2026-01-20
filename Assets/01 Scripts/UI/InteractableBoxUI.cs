
public class InteractableBoxUI : InteractableStateUI
{
    private bool _hasBeenOpened;

    public bool HasBeenOpened
    {
        get { return _hasBeenOpened; }
        set { _hasBeenOpened = value; }
    }

    private void Awake()
    {
        _hasBeenOpened = false;
    }

    public override void Selected()
    {
        base.Selected();

        _stateImage.sprite = GameResources.Instance.OpenableBoxSprite;
    }

    public override void Deselected()
    {
        base.Deselected();

        if (_hasBeenOpened)
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
