using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _countText;
    [SerializeField] private ItemInfoUI _infoUI;

    private UIManager _uiManager;
    private ItemSlot _itemSlot;
    private Inventory _inventory;
    private Image _image;
    private Transform _originParent;
    private Vector2 _originAncghoredPos;

    private float _lastClickTime;

    private const float _doubleClickThreshold = 0.25f;

    public ItemSlot Slot
    {
        get { return _itemSlot; }
        set
        {
            _itemSlot = value;

            if (_image == null)
                _image = GetComponent<Image>();

            if (_uiManager == null)
                _uiManager = UIManager.Instance;

            RefreshUI();
        }
    }

    private void Awake()
    {
        _image = GetComponent<Image>();
        _inventory = GameManager.Instance.Inventory;
        _originParent = transform.parent;
        _originAncghoredPos = ((RectTransform)transform).anchoredPosition;
        _uiManager = UIManager.Instance;
    }


    #region Drag And Drop

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_itemSlot == null || _itemSlot.Quantity == 0)
            return;

        transform.SetParent(_uiManager.DragCanvasTransform);

        _image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
            return;

        ItemSlotUI startUI = eventData.pointerDrag.GetComponent<ItemSlotUI>();

        if (startUI == null)
            return;
        if (startUI == this)
        {
            _infoUI.ShowUI();
            return;
        }

        Item startItem = startUI._itemSlot.CurrentItem;
        Item endItem = _itemSlot.CurrentItem;

        // 같은 ID 일 경우 개수 합치기
        if (startItem != null
            && endItem != null
            && startItem.ID == endItem.ID)
        {
            int remainItemCount = _itemSlot.AddItem(startItem, startUI._itemSlot.Quantity);

            // swap
            if (remainItemCount == startUI._itemSlot.Quantity)
            {
                SwapItem(startUI);
            }
            else
            {
                int subtractCount = startUI._itemSlot.Quantity - remainItemCount;
                startUI._itemSlot.SubtractItem(subtractCount);
            }
        }
        else if(startUI._itemSlot.CurrentItem != null)
            SwapItem(startUI);

        if (_itemSlot.CurrentItem != null)
            _infoUI.ShowUI();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.localPosition = Vector3.zero;
        _image.raycastTarget = true;

        transform.SetParent(_originParent);
        ((RectTransform)transform).anchoredPosition = _originAncghoredPos;
    }

    private void SwapItem(ItemSlotUI target)
    {
        Item tempItem = _itemSlot.CurrentItem;
        int tempQauntity = _itemSlot.Quantity;

        _itemSlot.SubtractItem(_itemSlot.Quantity);
        _itemSlot.AddItem(target._itemSlot.CurrentItem, target._itemSlot.Quantity);

        target._itemSlot.SubtractItem(target._itemSlot.Quantity);
        target._itemSlot.AddItem(tempItem, tempQauntity);
    }

    #endregion Drag And Drop



    #region Double Click

    public void OnPointerClick(PointerEventData eventData)
    {
        _uiManager.CloseSlotMenu();

        if (_itemSlot.CurrentItem == null)
            return;

        if (eventData.button == PointerEventData.InputButton.Right)
            OpenSlotMenu();


        if (Time.unscaledTime - _lastClickTime <= _doubleClickThreshold)
        {
            OnDoubleClick();
            _lastClickTime = 0f;
        }
        else
            _lastClickTime = Time.unscaledTime;
    }

    private void OnDoubleClick()
    {
        if (_itemSlot.CurrentItem == null
            || GameManager.Instance.CurrentOpenBox == null)
            return;

        _infoUI.HideUI();

        if (_itemSlot.Type == SlotType.INVENTORY)
        {
            TryMoveToBoxByDoubleClick();
        }
        else
        {
            TryMoveToInventoryByDoubleClick();
        }
    }

    private void TryMoveToInventoryByDoubleClick()
    {
        if (_inventory.TryAddItem(_itemSlot.CurrentItem, _itemSlot.Quantity))
        {
            _itemSlot.SubtractItem(_itemSlot.Quantity);
        }
        else // TODO : 빈공간이 없습니다 UI 표시
        {

        }
    }

    private void TryMoveToBoxByDoubleClick()
    {
        if (_itemSlot.CurrentItem.Type != ItemType.Gun)
        {
            // 같은 ID의 아이템이 있을 경우
            for (int i = 0; i < GameManager.Instance.BoxSlotNum; ++i)
            {
                ItemSlot targetSlot = GameManager.Instance.BoxItemSlots[i].GetComponent<ItemSlotUI>()._itemSlot;

                if (targetSlot.CurrentItem != null &&
                    targetSlot.CurrentItem.ID == _itemSlot.CurrentItem.ID)
                {
                    int remainAmount = targetSlot.AddItem(_itemSlot.CurrentItem, _itemSlot.Quantity);

                    _itemSlot.SubtractItem(_itemSlot.Quantity - remainAmount);

                    if (remainAmount == 0)
                        return;
                }
            }
        }

        // 그렇지 않을 경우
        for (int i = 0; i < GameManager.Instance.BoxSlotNum; ++i)
        {
            ItemSlot targetSlot = GameManager.Instance.BoxItemSlots[i].GetComponent<ItemSlotUI>()._itemSlot;

            if (targetSlot.CurrentItem == null)
            {
                targetSlot.AddItem(_itemSlot.CurrentItem, _itemSlot.Quantity);

                _itemSlot.SubtractItem(_itemSlot.Quantity);

                return;
            }
        }
    }

    #endregion Double Click


    #region Hover

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_itemSlot != null && _itemSlot.CurrentItem != null)
            _infoUI.ShowUI();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _infoUI.HideUI();
    }

    #endregion Hover


    public void RefreshUI()
    {
        if (_itemSlot.CurrentItem != null)
        {
            _image.sprite = ItemSpriteDictionary.Instance.GetItemSprite(_itemSlot.CurrentItem.ID);
            _uiManager.ChangeImageAlpha(_image, true);
        }
        else
        {
            _image.sprite = null;
            _uiManager.ChangeImageAlpha(_image, false);
        }

        ChangeTexts();
    }

    public void OpenSlotMenu()
    {
        if (_itemSlot.CurrentItem == null)
            return;

        _uiManager.OpenSlotMenu(_itemSlot, transform.position);
    }

    private void ChangeTexts()
    {
        if (_itemSlot.CurrentItem == null)
        {
            _nameText.text = string.Empty;
            _countText.text = string.Empty;
            return;
        }

        _nameText.text = _itemSlot.CurrentItem.Name;
        if (_itemSlot.Quantity > 1)
        {
            _countText.text = _itemSlot.Quantity.ToString();
        }
        else if (_itemSlot.Quantity == 1)
        {
            _countText.text = string.Empty;
        }
    }

    public void SetInfoUI(Item item)
    {
        _infoUI.SetInfoUI(item);
    }
}
