using UnityEngine;
using UnityEngine.UI;

public class UIManager : SingletonMonoBehaviour<UIManager>
{
    [SerializeField] private Image[] _boxSlotsImage;
    [SerializeField] private GameObject _slotMenuUI;

    [SerializeField] private Vector3 _inventorySlotMenuOffset;
    [SerializeField] private Vector3 _boxSlotMenuOffset;

    [SerializeField] private GameObject _equipButton;
    [SerializeField] private GameObject _unloadButton;
    [SerializeField] private GameObject _useButton;
    [SerializeField] private GameObject _splitButton;
    [SerializeField] private GameObject _discardButton;

    [SerializeField] private ItemSplitUI _splitUI;
    [SerializeField] private Transform _dragCanvasTransform;

    private ItemSlot _currentSlot;

    public Transform DragCanvasTransform { get { return _dragCanvasTransform; } }

    protected override void Awake()
    {
        base.Awake();

        _currentSlot = new ItemSlot();
    }

    public void ChangeImageAlpha(Image image, bool showImage)
    {
        Color color = image.color;
        color.a = showImage ? 255f : 0f;
        image.color = color;
    }

    public void OpenSlotMenu(ItemSlot slot, Vector3 pos)
    {
        _slotMenuUI.transform.position = pos;

        if (slot.Type == SlotType.INVENTORY)
            _slotMenuUI.transform.position += _inventorySlotMenuOffset;
        else
            _slotMenuUI.transform.position += _boxSlotMenuOffset;

        if (IsUpperHalf(_slotMenuUI.transform.position))
        {
            // TODO
        }

        _currentSlot = slot;
        ShowButtonsByItemtype();

        _slotMenuUI.SetActive(true);
    }

    public void CloseSlotMenu()
    {
        _slotMenuUI.SetActive(false);
    }

    private bool IsUpperHalf(Vector3 pos)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(pos);
        return screenPos.y > Screen.height * 0.5f;
    }

    private void ShowButtonsByItemtype()
    {
        string itemType = _currentSlot.CurrentItem.Type;

        // Temp
        _equipButton.SetActive(false);
        _unloadButton.SetActive(false);
        _useButton.SetActive(false);
        _splitButton.SetActive(false);
        _discardButton.SetActive(false);

        if (itemType == ItemType.Gun)
        {
            _equipButton.SetActive(true);
            _unloadButton.SetActive(true);
        }
        else if (itemType == ItemType.Medicine || itemType == ItemType.Food)
        {
            _useButton.SetActive(true);
            _splitButton.SetActive(true);
        }
        else
        {
            _splitButton.SetActive(true);
        }

        if (_currentSlot.Type == SlotType.INVENTORY)
        {
            _discardButton.SetActive(true);

            if (GameManager.Instance.Inventory.FindFirstEmptySlot() == -1
                || _currentSlot.Quantity < 2)
                _splitButton.SetActive(false);
        }
        else if(_currentSlot.Type == SlotType.BOX)
        {
            if (GameManager.Instance.CurrentBox.FindFirstEmptySlot() == -1 
                || _currentSlot.Quantity < 2)
                _splitButton.SetActive(false);
        }
    }

    #region On Button Click
    public void OnSplitButtonClick()
    {
        _splitUI.CurrentSlot = _currentSlot;
        _splitUI.gameObject.SetActive(true);
        CloseSlotMenu();
    }

    public void OnDiscardButtonClick()
    {
        GameManager.Instance.CreateDropItemObject(_currentSlot.CurrentItem, _currentSlot.Quantity);

        _currentSlot.SubtractItem(_currentSlot.Quantity);
        CloseSlotMenu();
    }

    #endregion On Button Click
}
