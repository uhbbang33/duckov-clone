
public class InteractableBoxUI : InteractableStateUI
{
    private bool _isOpened;

    private void Awake()
    {
        _isOpened = false;
    }

    public override void Deselected()
    {
        base.Deselected();

        if (_isOpened)
            _stateImage.sprite = GameResources.Instance.OpenedBoxSprite;
    }
}
