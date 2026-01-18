using TMPro;
using UnityEngine;

public class DropItemUI : InteractableStateUI
{
    [SerializeField] private TextMeshProUGUI _textMesh;
    private DroppedItem _droppedItem;

    private void Awake()
    {
        _droppedItem = GetComponent<DroppedItem>();
        _droppedItem.FinshInitialize += InitText;
    }

    private void InitText()
    {
        _textMesh.text = _droppedItem.CurrentItem.Name;
    }

    public override void Selected()
    {
        base.Selected();

        _stateImage.sprite = GameResources.Instance.OpenableBoxSprite;
    }

    public override void Deselected()
    {
        base.Deselected();

        _stateImage.sprite = GameResources.Instance.UnopenedBoxSprite;
    }

    public override void OnInteract()
    {
        base.OnInteract();

        _droppedItem.OnInteract();
    }
}
