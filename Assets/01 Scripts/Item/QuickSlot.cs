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

    private InputActions _inputActions;
    private ItemSlotUI _linkedInventorySlotUI;
    private UIManager _uiManager;

    private void Start()
    {
        _inputActions = GameManager.Instance.PlayerObject.GetComponent<Player>().Actions;

        SubscribeInputEvent(_quickSlotNum);

        _uiManager = UIManager.Instance;
        _uiManager.ChangeImageAlpha(_iconImage, false);
    }

    // 게임 종료까지 유지
    private void SubscribeInputEvent(int num)
    {
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
    }

    private void UseQuickSlotItem(InputAction.CallbackContext context)
    {
        if (_linkedInventorySlotUI != null)
            _linkedInventorySlotUI.Slot.UseItem();
    }

    public void RefreshUI()
    {
        if(_linkedInventorySlotUI == null)
        {
            _nameText.text = "";
            _countText.text = "";
            _iconImage.sprite = null;
            _durabilityUI.SetActive(false);
            _countUI.SetActive(false);
            _uiManager.ChangeImageAlpha(_iconImage, false);
            return;
        }

        _uiManager.ChangeImageAlpha(_iconImage, true);
        ItemSlot linkedItemSlot = _linkedInventorySlotUI.Slot;

        _nameText.text = _linkedInventorySlotUI.NameText;
        _iconImage.sprite = _linkedInventorySlotUI.IconImageSprite;

        // TODO : durability, count 활성화 비활성화
        _durabilitySlider.value = _linkedInventorySlotUI.DurabilitySliderValue;
        _countText.text = _linkedInventorySlotUI.CountText;
    }

    public void LinkToInventorySlotUI(ItemSlotUI inventorySlotUI)
    {
        if(_linkedInventorySlotUI != null)
        {
            _linkedInventorySlotUI.RemoveQuickSlotLink();
        }

        _linkedInventorySlotUI = inventorySlotUI;
        RefreshUI();

        if (_linkedInventorySlotUI != null)
        {
            _linkedInventorySlotUI.AddQuickSlotLink(this);
        }
    }

    public void UnLinkInventorySlotUI()
    {
        if(_linkedInventorySlotUI != null)
        {
            _linkedInventorySlotUI.RemoveQuickSlotLink();
            _linkedInventorySlotUI = null;
            RefreshUI();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_linkedInventorySlotUI != null)
        {
            _linkedInventorySlotUI.OnBeginDrag(eventData);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_linkedInventorySlotUI != null)
        {
            _linkedInventorySlotUI.OnDrag(eventData);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        ItemSlotUI startSlotUI = eventData.pointerDrag?.GetComponent<ItemSlotUI>();
        if (startSlotUI == null)
            return;

        ItemSlot startSlot = startSlotUI.Slot;

        if ((startSlot.CurrentItem.Type != ItemType.Medicine
           && startSlot.CurrentItem.Type != ItemType.Food) 
           || startSlot.Type != SlotType.INVENTORY)
            return;

        ItemSlotUI tempSlot = _linkedInventorySlotUI;
        LinkToInventorySlotUI(startSlotUI);

        QuickSlot otherQuickSlot = startSlotUI.LinkedQuickSlot;
        if(otherQuickSlot != null && otherQuickSlot != this)
        {
            otherQuickSlot.LinkToInventorySlotUI(tempSlot);
        }

        QuickSlotManager.Instance.AddToQuickSlot((int)_linkedInventorySlotUI.Slot.CurrentItem.ID, _quickSlotNum);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_linkedInventorySlotUI != null)
        {
            _linkedInventorySlotUI.OnEndDrag(eventData);
        }
    }
}
