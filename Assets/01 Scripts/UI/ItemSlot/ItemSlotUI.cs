using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] protected TextMeshProUGUI _nameText;
    [SerializeField] protected TextMeshProUGUI _countText;
    [SerializeField] private ItemInfoUI _infoUI;
    [SerializeField] protected Image _iconImage;
    [SerializeField] private GameObject _nameUI;
    [SerializeField] private GameObject _durabilityUI;
    [SerializeField] private GameObject _countUI;
    [SerializeField] private Slider _durabilitySlider;

    protected UIManager _uiManager;
    protected ItemSlot _itemSlot;
    protected Inventory _inventory;
    private Transform _originParent;
    private Vector2 _originAncghoredPos;
    private RectTransform _rect;
    private QuickSlot _linkedQuickSlot;
    private FieldManager _gameManager;


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
        set
        {
            if(_linkedQuickSlot != null)
                _linkedQuickSlot.LinkedInventorySlotUI = null;

            _linkedQuickSlot = value;

            if (value == null || _itemSlot.Type != SlotType.INVENTORY)
                return;

            value.LinkedInventorySlotUI = this;
        }
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
        _gameManager = FieldManager.Instance;
        _originParent = transform.parent;
        _originAncghoredPos = ((RectTransform)transform).anchoredPosition;
    }

    protected virtual void Start()
    {
        _inventory = _gameManager.Inventory;
        _uiManager = UIManager.Instance;
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

    public void OnDrag(PointerEventData eventData)
    {
        if (_itemSlot.CurrentItem == null)
            return;
        transform.position = eventData.position;
    }

    public virtual void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
            return;

        if (_itemSlot == null || _itemSlot.Type == SlotType.SHOP)
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

            if (startUI._linkedQuickSlot != null
                && Slot.Type == SlotType.STORAGE)
                startUI._linkedQuickSlot.UnlinkInventorySlotUI(startItem.ID);

            // 같은 ID 일 경우 개수 합치기
            if (startItem != null
                && endItem != null
                && startItem.ID == endItem.ID)
            {
                int remainItemCount = startUI._itemSlot.Quantity;

                _itemSlot.AddItem(startItem, ref remainItemCount);

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

                    if (startUI._itemSlot.CurrentItem == null
                        && startUI._linkedQuickSlot != null)
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

    protected void SwapItem(ItemSlotUI target)
    {
        if(target._itemSlot.Type == SlotType.EQUIP
            && _itemSlot.CurrentItem != null)
        {
            if (_itemSlot.CurrentItem.Type != ItemType.Gun)
                return;
        }

        Item tempItem = _itemSlot.CurrentItem;
        int tempQuantity = _itemSlot.Quantity;
        int targetQuantity = target._itemSlot.Quantity;

        _itemSlot.SubtractItem(_itemSlot.Quantity);
        _itemSlot.AddItem(target._itemSlot.CurrentItem, ref targetQuantity);

        target._itemSlot.SubtractItem(target._itemSlot.Quantity);
        target._itemSlot.AddItem(tempItem, ref tempQuantity);
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

        if (_itemSlot.Type == SlotType.INVENTORY
               || _itemSlot.Type == SlotType.EQUIP)
        {
            if (_gameManager.CurrentOpenBox != null)
                TryMoveToContainerByDoubleClick(SlotType.BOX);
            else if (_gameManager.IsStorageOpened)
                TryMoveToContainerByDoubleClick(SlotType.STORAGE);
            else if (_gameManager.IsShopOpened)
                SellItem();
        }
        else if (_itemSlot.Type == SlotType.BOX 
            || _itemSlot.Type == SlotType.STORAGE)
        {
            TryMoveToInventoryByDoubleClick();
        }
    }

    private void TryMoveToInventoryByDoubleClick()
    {
        int remainQuantity = _itemSlot.Quantity;

        _inventory.TryAddItem(_itemSlot.CurrentItem, ref remainQuantity);
        _itemSlot.SubtractItem(_itemSlot.Quantity - remainQuantity);
    }

    private void TryMoveToContainerByDoubleClick(SlotType openContainerType)
    {
        List<ItemSlot> targetSlots = GetContainerSlots(openContainerType);

        // 같은 ID의 아이템이 있을 경우
        foreach (ItemSlot targetSlot in targetSlots)
        {
            if (targetSlot.CurrentItem == null ||
                targetSlot.CurrentItem.ID != _itemSlot.CurrentItem.ID)
                continue;

            int remainAmount = _itemSlot.Quantity;
            targetSlot.AddItem(_itemSlot.CurrentItem, ref remainAmount);
            _itemSlot.SubtractItem(_itemSlot.Quantity - remainAmount);

            if (remainAmount == 0)
            {
                _linkedQuickSlot?.UnlinkInventorySlotUI(targetSlot.CurrentItem.ID);
                return;
            }
        }

        // 그렇지 않을 경우
        foreach (ItemSlot targetSlot in targetSlots)
        {
            if (targetSlot.CurrentItem != null)
                continue;

            int remainAmount = _itemSlot.Quantity;
            targetSlot.AddItem(_itemSlot.CurrentItem, ref remainAmount);
            _itemSlot.SubtractItem(_itemSlot.Quantity - remainAmount);
            _linkedQuickSlot?.UnlinkInventorySlotUI(targetSlot.CurrentItem.ID);
            return;
        }
    }

    private void SellItem()
    {
        // 아이템 value 값만큼 재화 증가
        _inventory.ChangeMoney((int)_itemSlot.CurrentItem.Value, _itemSlot.Quantity, true);

        // 가방에서 아이템 삭제
        _linkedQuickSlot?.UnlinkInventorySlotUI(_itemSlot.CurrentItem.ID);
        _itemSlot.SubtractItem(_itemSlot.Quantity);
    }

    private List<ItemSlot> GetContainerSlots(SlotType openContainerType)
    {
        GameObject[] slotObjects = openContainerType switch
        {
            SlotType.BOX => _gameManager.BoxItemSlots,
            SlotType.STORAGE => _gameManager.StorageItemSlots,
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
        if (_itemSlot.CurrentItem == null
            || _itemSlot.Type == SlotType.SHOP)
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
            //_nameText.text = string.Empty;
            //_countText.text = string.Empty;
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
            
            if(usableItem.DurabilityCost != Durability.MaxDurability)
                _durabilityUI.SetActive(true);
        }

        if (!_durabilityUI.activeSelf && _itemSlot.Quantity > 1)
            _countUI.SetActive(true);
    }

    public void ChangeDurabilitySliderValue(int current, int max)
    {
        _durabilitySlider.value = (float)current / (float)max;
        RefreshUI();
    }

    public void ChangeImageAlpha(bool showImage)
    {
        _uiManager.ChangeImageAlpha(_iconImage, showImage);
    }
}
