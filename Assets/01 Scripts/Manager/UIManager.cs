using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : SingletonMonoBehaviour<UIManager>
{
    [Space(10)]
    [Header("Offset")]
    [SerializeField] private Vector3 _inventorySlotMenuOffset;
    [SerializeField] private Vector3 _boxSlotMenuOffset;

    [Space(10)]
    [Header("Button")]
    [SerializeField] private GameObject _equipButton;
    [SerializeField] private GameObject _unloadButton;
    [SerializeField] private GameObject _useButton;
    [SerializeField] private GameObject _splitButton;
    [SerializeField] private GameObject _discardButton;

    [Space(10)]
    [Header("Text")]
    [SerializeField] private TextMeshProUGUI _boxItemCountText;
    [SerializeField] private TextMeshProUGUI _inventoryItemCountText;

    [Space(20)]
    [SerializeField] private ItemSplitUI _splitUI;
    [SerializeField] private Transform _dragCanvasTransform;
    [SerializeField] private GameObject _slotMenuUI;
    [SerializeField] private GameObject _buttonsObject;

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
        _buttonsObject.transform.position = pos;

        if (slot.Type == SlotType.INVENTORY)
            _buttonsObject.transform.position += _inventorySlotMenuOffset;
        else
            _buttonsObject.transform.position += _boxSlotMenuOffset;

        if (IsUpperHalf(_buttonsObject.transform.position))
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

    public void ChangeBoxItemCountText(int itemCnt, int maxCnt)
    {
        _boxItemCountText.text = "상자 (" + itemCnt + " / " + maxCnt + ")";
    }

    public void ChangeInventoryItemCountText(int itemCnt, int maxCnt)
    {
        _inventoryItemCountText.text 
            = "가방 (" + itemCnt + " / " + maxCnt + ")";
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
        if (GameManager.Instance.CreateDropItemObject(_currentSlot.CurrentItem, _currentSlot.Quantity))
        {
            _currentSlot.SubtractItem(_currentSlot.Quantity);
        }
        else // TODO: 버릴 수 없습니다 UI
        {
            Debug.Log("버릴 수 없습니다.");
        }

        CloseSlotMenu();
    }

    public void OnUsebuttonClick()
    {
        UsableItem item = _currentSlot.CurrentItem as UsableItem;

        if(item.DurabilityCost > 0)
        {
            // item의 내구도 감소
        }

        GameManager.Instance.PlayerObject.GetComponent<Player>().UseItem(item);

        _currentSlot.SubtractItem();

        CloseSlotMenu();
    }

    #endregion On Button Click
}
