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
    [SerializeField] protected Image _iconImage;
    [SerializeField] private GameObject _durabilityUI;
    [SerializeField] private GameObject _countUI;
    [SerializeField] private Slider _durabilitySlider;

    protected UIManager _uiManager;
    protected ItemSlot _itemSlot;
    private Inventory _inventory;
    private Transform _originParent;
    private Vector2 _originAncghoredPos;
    private RectTransform _rect;
    private QuickSlot _linkedQuickSlot;


    private float _lastClickTime;

    private const float _doubleClickThreshold = 0.25f;

    public ItemSlot Slot
    {
        get { return _itemSlot; }
        set
        {
            _itemSlot = value;

            if (_uiManager == null)
                _uiManager = UIManager.Instance;

            RefreshUI();
        }
    }

    public QuickSlot LinkedQuickSlot
    {
        get { return _linkedQuickSlot; }
    }

    public Sprite IconImageSprite
    {
        get { return _iconImage.sprite; }
    }

    public string NameText
    {
        get { return _nameText.text; }
    }

    public string CountText
    {
        get { return _countText.text; }
    }

    public float DurabilitySliderValue
    {
        get { return _durabilitySlider.value; }
    }

    private void Awake()
    {
        _originParent = transform.parent;
        _originAncghoredPos = ((RectTransform)transform).anchoredPosition;
    }

    protected virtual void Start()
    {
        _inventory = GameManager.Instance.Inventory;
        _uiManager = UIManager.Instance;
        ChangeImageAlpha(false);
    }

    #region Drag And Drop

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        if (_itemSlot == null || _itemSlot.Quantity == 0)
            return;

        transform.SetParent(_uiManager.DragCanvasTransform);

        _iconImage.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public virtual void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
            return;

        ItemSlotUI startUI = eventData.pointerDrag.GetComponent<ItemSlotUI>();

        if (startUI != null)
        {
            if (!CheckTypeBeforeDrop(startUI._itemSlot))
                return;

            if (startUI == this)
            {
                if (_infoUI != null)
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
                    SwapQuickSlot(startUI, _linkedQuickSlot);
                    SwapItem(startUI);
                }
                else
                {
                    int subtractCount = startUI._itemSlot.Quantity - remainItemCount;
                    startUI._itemSlot.SubtractItem(subtractCount);

                    if(startUI._itemSlot.CurrentItem == null)
                    {
                        startUI._linkedQuickSlot.UnlinkInventorySlotUI(startItem.ID);
                    }
                }
            }
            else if (startItem != null)
            {
                SwapQuickSlot(startUI, _linkedQuickSlot);
                SwapItem(startUI);
            }

            if (_itemSlot.CurrentItem != null && _infoUI != null)
                _infoUI.ShowUI();

            return;
        }


        QuickSlot startQuickSlot = eventData.pointerDrag?.GetComponent<QuickSlot>();

        if (startQuickSlot != null)
        {
            if (startQuickSlot == _linkedQuickSlot
                || _itemSlot.Type != SlotType.INVENTORY)
                return;

            ItemSlotUI startInventorySlot = startQuickSlot.LinkedInventorySlotUI;
            if (startInventorySlot == null)
                return;

            QuickSlot currentQuickSlot = _linkedQuickSlot;

            SwapItem(startInventorySlot);
            startQuickSlot.LinkToInventorySlotUI(this);

            if (currentQuickSlot != null)
                currentQuickSlot.LinkToInventorySlotUI(startInventorySlot);
        }

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.localPosition = Vector3.zero;
        _iconImage.raycastTarget = true;

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

    private void SwapQuickSlot(ItemSlotUI startUI, QuickSlot currentQuick)
    {
        if (_itemSlot.Type == SlotType.BOX)
        {
            if (startUI._linkedQuickSlot != null)
            {
                startUI._linkedQuickSlot.UnlinkInventorySlotUI(startUI.Slot.CurrentItem.ID);
            }
        }

        QuickSlot startQuick = startUI._linkedQuickSlot;

        if (startQuick != null)
            startQuick.LinkToInventorySlotUI(this);

        if (currentQuick != null)
            currentQuick.LinkToInventorySlotUI(startUI);
    }

    protected virtual bool CheckTypeBeforeDrop(ItemSlot startSlot)
    {
        return true;
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

        if (_infoUI != null)
            _infoUI.HideUI();

        if (_itemSlot.Type == SlotType.INVENTORY)
        {
            TryMoveToBoxByDoubleClick();
        }
        else if(_itemSlot.Type == SlotType.BOX)
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
                ItemSlot targetSlot = GameManager.Instance.BoxItemSlots[i].GetComponentInChildren<ItemSlotUI>()._itemSlot;

                if (targetSlot.CurrentItem != null &&
                    targetSlot.CurrentItem.ID == _itemSlot.CurrentItem.ID)
                {
                    int remainAmount = targetSlot.AddItem(_itemSlot.CurrentItem, _itemSlot.Quantity);

                    _itemSlot.SubtractItem(_itemSlot.Quantity - remainAmount);

                    if (remainAmount == 0)
                    {
                        if (_linkedQuickSlot != null)
                            _linkedQuickSlot.UnlinkInventorySlotUI(targetSlot.CurrentItem.ID);

                        return;
                    }
                }
            }
        }

        // 그렇지 않을 경우
        for (int i = 0; i < GameManager.Instance.BoxSlotNum; ++i)
        {
            ItemSlot targetSlot = GameManager.Instance.BoxItemSlots[i].GetComponentInChildren<ItemSlotUI>()._itemSlot;

            if (targetSlot.CurrentItem == null)
            {
                targetSlot.AddItem(_itemSlot.CurrentItem, _itemSlot.Quantity);

                _itemSlot.SubtractItem(_itemSlot.Quantity);

                if (_linkedQuickSlot != null)
                    _linkedQuickSlot.UnlinkInventorySlotUI(targetSlot.CurrentItem.ID);

                return;
            }
        }
    }

    #endregion Double Click


    #region Hover

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_itemSlot != null && _itemSlot.CurrentItem != null
            && _infoUI != null)
            _infoUI.ShowUI();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_infoUI != null)
            _infoUI.HideUI();
    }

    #endregion Hover


    public virtual void RefreshUI()
    {
        Item item = _itemSlot.CurrentItem;

        if (item != null)
        {
            _iconImage.sprite = ItemSpriteDictionary.Instance.GetItemSprite(item.ID);
            ChangeImageAlpha(true);
        }
        else
        {
            _iconImage.sprite = null;
            ChangeImageAlpha(false);
        }

        if (_infoUI != null)
            _infoUI.SetInfoUI(item);

        ChangeTexts();
        SetDurabilityOrCountUI(item);

        if(_rect == null)
            _rect = GetComponent<RectTransform>();
        // Vertical Layout Group 재정렬
        LayoutRebuilder.ForceRebuildLayoutImmediate(_rect);

        if(_linkedQuickSlot != null)
        {
            _linkedQuickSlot.RefreshUI();
        }
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

    private void SetDurabilityOrCountUI(Item item)
    {
        _durabilityUI.SetActive(false);
        _countUI.SetActive(false);

        if (item == null)
            return;

        if (item.Type == ItemType.Food || item.Type == ItemType.Medicine)
        {
            UsableItem usableItem = item as UsableItem;
            
            if(usableItem.DurabilityCost != Durability.MaxDurability)
                _durabilityUI.SetActive(true);
        }

        if (!_durabilityUI.activeSelf)
            _countUI.SetActive(true);
    }

    public void ChangeDurabilitySliderValue(int current, int max)
    {
        _durabilitySlider.value = (float)current / (float)max;
    }

    public void ChangeImageAlpha(bool showImage)
    {
        _uiManager.ChangeImageAlpha(_iconImage, showImage);
    }

    public void LinkQuickSlot(QuickSlot quickSlot)
    {
        _linkedQuickSlot = quickSlot;
    }

    public void UnlinkQuickSlot()
    {
        _linkedQuickSlot = null;
    }
}
