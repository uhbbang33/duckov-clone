using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class QuickSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _countText;
    [SerializeField] protected Image _iconImage;
    [SerializeField] private GameObject _durabilityUI;
    [SerializeField] private GameObject _countUI;
    [SerializeField] private Slider _durabilitySlider;
    [SerializeField] private int _quickSlotNum;
    [SerializeField] private DefaultHUDSlotUI _defaultHUDSlotUI;

    private InputActions _inputActions;
    private ItemSlotUI _linkedInventorySlotUI;
    private ItemSlotUI _beginSlotUI;
    private UIManager _uiManager;
    private RectTransform _rect;
    private bool _isSubscribeInputEvent = false;

    public ItemSlotUI LinkedInventorySlotUI
    {
        get { return _linkedInventorySlotUI; }
        set { _linkedInventorySlotUI = value; }
    }

    private void Start()
    {
        Initialize();
        _uiManager.ChangeImageAlpha(_iconImage, false);

        RefreshUI();
    }

    private void Initialize()
    {
        if (_uiManager == null)
        {
            _uiManager = UIManager.Instance;
        }
        if (!_isSubscribeInputEvent)
        {
            SubscribeInputEvent(_quickSlotNum);
        }
    }

    // 게임 종료까지 유지
    private void SubscribeInputEvent(int num)
    {
        _inputActions = FieldManager.Instance.PlayerObject.GetComponent<Player>().Actions;

        switch (num)
        {
            case 3:
                _inputActions.Player.QuickSlot3.performed += UseQuickSlotItem;
                break;
            case 4:
                _inputActions.Player.QuickSlot4.performed += UseQuickSlotItem;
                break;
            case 5:
                _inputActions.Player.QuickSlot5.performed += UseQuickSlotItem;
                break;
            case 6:
                _inputActions.Player.QuickSlot6.performed += UseQuickSlotItem;
                break;
            case 7:
                _inputActions.Player.QuickSlot7.performed += UseQuickSlotItem;
                break;
            case 8:
                _inputActions.Player.QuickSlot8.performed += UseQuickSlotItem;
                break;
        }
        _isSubscribeInputEvent = true;
    }

    private void UseQuickSlotItem(InputAction.CallbackContext context)
    {
        if (_linkedInventorySlotUI != null)
            _linkedInventorySlotUI.Slot.UseItem();
    }

    public void RefreshUI()
    {
        Initialize();

        if (_linkedInventorySlotUI == null)
        {
            _nameText.text = "";
            _countText.text = "";
            _iconImage.sprite = null;
            _durabilityUI.SetActive(false);
            _countUI.SetActive(false);
            _uiManager.ChangeImageAlpha(_iconImage, false);
            _defaultHUDSlotUI.RefreshDefaultUHD(false);
            return;
        }

        _uiManager.ChangeImageAlpha(_iconImage, true);

        _nameText.text = _linkedInventorySlotUI.NameText;
        _iconImage.sprite = _linkedInventorySlotUI.IconImageSprite;

        SetDurabilityOrCountUI(_linkedInventorySlotUI.Slot.CurrentItem);

        _durabilitySlider.value = _linkedInventorySlotUI.DurabilitySliderValue;
        _countText.text = _linkedInventorySlotUI.CountText;


        if (_rect == null)
            _rect = GetComponent<RectTransform>();
        LayoutRebuilder.ForceRebuildLayoutImmediate(_rect);

        _defaultHUDSlotUI.RefreshDefaultUHD(true, _nameText.text, _countText.text, _iconImage.sprite, _durabilityUI.activeSelf, _countUI.activeSelf, _durabilitySlider.value);
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
        }

        if (!_durabilityUI.activeSelf)
            _countUI.SetActive(true);
    }

    public void LinkToInventorySlotUI(ItemSlotUI inventorySlotUI)
    {
        Initialize();

        if (inventorySlotUI == null)
            return;
        if (_linkedInventorySlotUI == inventorySlotUI)
            return;
        if (inventorySlotUI.LinkedQuickSlot == this)
            return;

        // 인벤토리 슬롯 연결 끊기
        if (inventorySlotUI.LinkedQuickSlot != null)
        {
            Item itemToUnlink = inventorySlotUI.Slot.CurrentItem;
            uint unlinkItemId = itemToUnlink != null ? itemToUnlink.ID : 0;

            inventorySlotUI.LinkedQuickSlot.UnlinkInventorySlotUI(unlinkItemId);
        }

        // 현재 퀵슬롯 연결 끊기
        if (_linkedInventorySlotUI != null)
        {
            Item currentItem = _linkedInventorySlotUI.Slot.CurrentItem;
            uint currentItemId = currentItem != null ? currentItem.ID : 0;

            UnlinkInventorySlotUI(currentItemId);
        }

        // 새로운 연결
        _linkedInventorySlotUI = inventorySlotUI;
        inventorySlotUI.LinkedQuickSlot = this;

        // Dictionary 업데이트
        Item inventoryItem = inventorySlotUI.Slot.CurrentItem;
        if (inventoryItem != null)
        {
            QuickSlotManager.Instance.AddDict((int)inventoryItem.ID, _quickSlotNum);
        }

        RefreshUI();
    }

    public void UnlinkInventorySlotUI(uint itemId)
    {
        if(_linkedInventorySlotUI != null)
        {
            if (itemId != 0)
                QuickSlotManager.Instance.RemoveDict((int)itemId);

            _linkedInventorySlotUI.LinkedQuickSlot = null;
            _linkedInventorySlotUI = null;

            RefreshUI();
        }
    }

    #region Drag And Drop

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_linkedInventorySlotUI != null)
        {
            _beginSlotUI = _linkedInventorySlotUI;
            _beginSlotUI.OnBeginDrag(eventData);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_beginSlotUI != null)
        {
            _beginSlotUI.OnDrag(eventData);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
            return;

        ItemSlotUI startSlotUI = eventData.pointerDrag?.GetComponent<ItemSlotUI>();

        // 시작이 인벤토리일 경우
        if (startSlotUI != null)
        {
            ItemSlot startSlot = startSlotUI.Slot;

            if (startSlot == null || startSlot.CurrentItem == null)
                return;

            if ((startSlot.CurrentItem.Type != ItemType.Medicine
               && startSlot.CurrentItem.Type != ItemType.Food)
               || startSlot.Type != SlotType.INVENTORY)
                return;


            LinkToInventorySlotUI(startSlotUI);

            return;
        }

        QuickSlot startQuickSlot = eventData.pointerDrag?.GetComponent<QuickSlot>();

        // 시작이 퀵슬롯일 경우
        if (startQuickSlot != null)
        {
            // start가 inventory에 연결 안되어 있으면 return
            if (startQuickSlot._linkedInventorySlotUI == null)
                return;

            // 현(end) 퀵슬롯이 인벤토리와 연결 안되어 있을 경우
            if(_linkedInventorySlotUI == null)
            {
                // start의 연결 슬롯과 연결
                LinkToInventorySlotUI(startQuickSlot._linkedInventorySlotUI);
            }
            else // 연결되어 있는 경우
            {
                ItemSlotUI tempSlot = _linkedInventorySlotUI;

                LinkToInventorySlotUI(startQuickSlot._linkedInventorySlotUI);
                startQuickSlot.LinkToInventorySlotUI(tempSlot);
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_beginSlotUI != null)
        {
            _beginSlotUI.OnEndDrag(eventData);
            _beginSlotUI = null;
        }
    }

    #endregion Drag And Drop
}
