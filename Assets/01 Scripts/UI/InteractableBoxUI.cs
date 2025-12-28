using UnityEngine;
using UnityEngine.UI;

public class InteractableBoxUI : InteractableStateUI
{
    [SerializeField] private Sprite _unopenedBoxSprite;
    [SerializeField] private Sprite _openedBoxSprite;
    [SerializeField] private Sprite _openableBoxSprite;

    private bool _isOpened;

    private void Awake()
    {
        _isOpened = false;
    }

    public override void Selected()
    {
        _stateImage.sprite = _openableBoxSprite;
    }

    public override void Deselected()
    {
        if (_isOpened)
            _stateImage.sprite = _openedBoxSprite;
        else
            _stateImage.sprite = _unopenedBoxSprite;
    }
}
