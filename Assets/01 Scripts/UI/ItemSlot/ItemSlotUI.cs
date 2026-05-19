using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(DynamicFontSize))]
public class ItemSlotUI : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{ 
    [SerializeField] protected TextMeshProUGUI _nameText;
    [SerializeField] protected TextMeshProUGUI _countText;
    [SerializeField] protected ItemInfoUI _infoUI;
    [SerializeField] protected Image _iconImage;
    [SerializeField] private GameObject _nameUI;
    [SerializeField] private GameObject _durabilityUI;
    [SerializeField] private GameObject _countUI;
    [SerializeField] private Slider _durabilitySlider;

    protected UIManager _uiManager;
    protected FieldManager _fieldManager;
    protected BunkerManager _bunkerManager;
    protected SoundManager _soundManager;
    protected ItemSlot _itemSlot;
    protected Inventory _inventory;
    private Transform _originParent;
    private Vector2 _originAncghoredPos;
    private RectTransform _rect;
    private DynamicFontSize _dynamicFontSize;

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

    public virtual QuickSlot LinkedQuickSlot
    {
        get; set;
    }

    public Sprite IconImageSprite => _iconImage.sprite;
    public string NameText =>  _nameText.text;
    public string CountText => _countText.text; 
    public float DurabilitySliderValue => _durabilitySlider.value;

    protected virtual void Awake()
    {
        _originParent = transform.parent;
        _originAncghoredPos = ((RectTransform)transform).anchoredPosition;

        _dynamicFontSize = GetComponent<DynamicFontSize>();
    }

    protected virtual void Start()
    {
        _inventory = GameManager.Instance.Inventory;
        _uiManager = UIManager.Instance;
        _fieldManager = FieldManager.Instance;
        _bunkerManager = BunkerManager.Instance;
        _soundManager = SoundManager.Instance;
    }

    #region Drag And Drop

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        if (_itemSlot == null 
            || _itemSlot.CurrentItem == null)
            return;

        transform.SetParent(_uiManager.DragCanvasTransform);

        _iconImage.raycastTarget = false;
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (_itemSlot.CurrentItem == null)
            return;
        transform.position = eventData.position;
    }

    public virtual void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null
            || _itemSlot == null)
            return;

        ItemSlotUI startUI = eventData.pointerDrag.GetComponent<ItemSlotUI>();

        if (startUI == null
            || !CheckTypeBeforeDrop(startUI._itemSlot))
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
            CombineItem(startUI);
        }
        else if (startItem != null)
        {
            SwapItem(startUI);
        }

        if (_itemSlot.CurrentItem != null && _infoUI != null)
            _infoUI.ShowUI();

        PlayPickItemSound();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.localPosition = Vector3.zero;
        _iconImage.raycastTarget = true;

        transform.SetParent(_originParent);
        ((RectTransform)transform).anchoredPosition = _originAncghoredPos;
    }

    protected virtual void CombineItem(ItemSlotUI startUI)
    {
        int remainItemCount = startUI._itemSlot.Quantity;

        Item startItem = startUI._itemSlot.CurrentItem;
        _itemSlot.AddItem(startItem, ref remainItemCount);

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

    protected virtual void SwapItem(ItemSlotUI target)
    {
        Item tempItem = _itemSlot.CurrentItem;
        int tempQuantity = _itemSlot.Quantity;
        int targetQuantity = target._itemSlot.Quantity;

        _itemSlot.SubtractItem(_itemSlot.Quantity);
        _itemSlot.AddItem(target._itemSlot.CurrentItem, ref targetQuantity);

        target._itemSlot.SubtractItem(target._itemSlot.Quantity);
        target._itemSlot.AddItem(tempItem, ref tempQuantity);
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

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (Time.unscaledTime - _lastClickTime <= _doubleClickThreshold)
            {
                OnDoubleClick();
                _lastClickTime = 0f;
            }
            else
                _lastClickTime = Time.unscaledTime;
        }
    }

    protected virtual void OnDoubleClick()
    {
        if (_itemSlot.CurrentItem == null)
            return;

        if (_infoUI != null)
            _infoUI.HideUI();
    }

    protected void TryMoveToInventoryByDoubleClick()
    {
        int remainQuantity = _itemSlot.Quantity;

        if (_inventory.TryAddItem(_itemSlot.CurrentItem, ref remainQuantity))
        {
            _itemSlot.SubtractItem(_itemSlot.Quantity - remainQuantity);
            PlayPickItemSound();
        }
    }

    protected List<ItemSlot> GetContainerSlots(SlotType openContainerType)
    {
        GameObject[] slotObjects = openContainerType switch
        {
            SlotType.BOX => _fieldManager.BoxItemSlots,
            SlotType.STORAGE => _bunkerManager.StorageItemSlots,
            _ => Array.Empty<GameObject>()
        };

        return slotObjects
            .Select(slot => slot.GetComponentInChildren<ItemSlotUI>()._itemSlot)
            .ToList();
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
            _nameUI.SetActive(false);
        }

        if (_infoUI != null)
            _infoUI.SetInfoUI(item, _itemSlot.Quantity);

        ChangeTexts();
        SetDurabilityOrCountUI(item);

        if (_rect == null)
            _rect = GetComponent<RectTransform>();
        // Vertical Layout Group 재정렬
        LayoutRebuilder.ForceRebuildLayoutImmediate(_rect);

        if(_dynamicFontSize != null)
            _dynamicFontSize.SetTextSize();
    }

    protected virtual void OpenSlotMenu()
    {
        if (_itemSlot.CurrentItem == null)
            return;

        _uiManager.OpenSlotMenu(_itemSlot, transform.position);
    }

    private void ChangeTexts()
    {
        if (_itemSlot.CurrentItem == null)
        {
            _nameUI.SetActive(false);
            _countUI.SetActive(false);
            _nameUI.SetActive(false);
            return;
        }

        _nameUI.SetActive(true);

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

            if (usableItem.DurabilityCost != Durability.MaxDurability)
                _durabilityUI.SetActive(true);

            ChangeDurabilitySliderValue(usableItem.CurrentDurability, Durability.MaxDurability);
        }

        if (!_durabilityUI.activeSelf && _itemSlot.Quantity > 1)
            _countUI.SetActive(true);
    }

    private void ChangeDurabilitySliderValue(int current, int max)
    {
        _durabilitySlider.value = (float)current / (float)max;
    }

    public void ChangeImageAlpha(bool showImage)
    {
        _uiManager.ChangeImageAlpha(_iconImage, showImage);
    }

    protected void PlayPickItemSound()
    {
        _soundManager.PlaySFXOneShot(SFXName.PickItem);
    }

    protected void PlaySellBuySound()
    {
        _soundManager.PlaySFXOneShot(SFXName.SellBuy);
    }
}
