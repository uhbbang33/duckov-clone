using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerClickHandler
{
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _countText;
    private ItemSlot _itemSlot;
    private Image _image;

    private float _lastClickTime;
    private bool _isInventorySlotUI;

    private const float _doubleClickThreshold = 0.25f;

    public ItemSlot Slot
    {
        get { return _itemSlot; }
        set
        {
            _itemSlot = value;

            if (_image == null)
                _image = GetComponent<Image>();
            
            RefreshUI();
        }
    }

    private void Awake()
    {
        _image = GetComponent<Image>();
        _isInventorySlotUI = transform.IsChildOf(GameManager.Instance.InventoryRoot);
    }


    #region Drag And Drop

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(_itemSlot == null || _itemSlot.Quantity == 0)
            return;

        transform.SetAsLastSibling();
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

        ItemSlotUI dragUI = eventData.pointerDrag.GetComponent<ItemSlotUI>();

        if (dragUI == null || dragUI == this)
            return;

        Item dragItem = dragUI._itemSlot.CurrentItem;
        Item dropItem = _itemSlot.CurrentItem;

        // 같은 ID 일 경우 개수 합치기
        if (dragItem != null && dropItem != null
            && dragItem.ID == dropItem.ID
            && dragItem.Type != ItemType.Gun)
        {
            int movedQuantity = dragUI._itemSlot.Quantity;

            _itemSlot.Quantity += movedQuantity;
            dragUI._itemSlot.SubtractItem(movedQuantity);

            if (dragUI._itemSlot.Quantity == 0 && dragUI._isInventorySlotUI)
            {
                GameManager.Instance.Inventory.RemoveItemSlot(dragItem.ID);
            }

            RefreshUI();
            dragUI.RefreshUI();

            return;
        }

        SwapItem(dragUI);

        // update inventory
        if (dragUI._isInventorySlotUI && dragUI._itemSlot.CurrentItem == null)
        {
            GameManager.Instance.Inventory.RemoveItemSlot(dragItem.ID);
        }

        if (_isInventorySlotUI && _itemSlot.CurrentItem != null)
        {
            GameManager.Instance.Inventory.TryAddItemByDragAndDrop(_itemSlot.CurrentItem.ID);
        }

        RefreshUI();
        dragUI.RefreshUI();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.localPosition = Vector3.zero;
        _image.raycastTarget = true;
    }

    private void SwapItem(ItemSlotUI target)
    {
        (target._itemSlot.CurrentItem, _itemSlot.CurrentItem) = (_itemSlot.CurrentItem, target._itemSlot.CurrentItem);

        (target._itemSlot.Quantity, _itemSlot.Quantity) = (_itemSlot.Quantity, target._itemSlot.Quantity);
    }

    #endregion Drag And Drop

   

    #region Double Click

    public void OnPointerClick(PointerEventData eventData)
    {
        // UI Slot 말고, 어디든 클릭하면 SLot Menu가 닫혀야 함
        UIManager.Instance.CloseSlotMenu();

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
        if (_itemSlot.CurrentItem == null)
            return;

        else if (_isInventorySlotUI)
        {
            TryMoveToBox();
        }
        else
        {
            TryMoveToInventory();
        }
    }

    private void TryMoveToInventory()
    {
        if(GameManager.Instance.Inventory.TryAddItemByDoubleClick(_itemSlot.CurrentItem, _itemSlot.Quantity))
        {
            RemoveItem();
        }
    }

    private void TryMoveToBox()
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
                    GameManager.Instance.Inventory.RemoveItemSlot(_itemSlot.CurrentItem.ID);

                    targetSlot.AddItem(_itemSlot.CurrentItem, _itemSlot.Quantity);

                    targetSlot.UI.RefreshUI();
                    RemoveItem();

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
                GameManager.Instance.Inventory.RemoveItemSlot(_itemSlot.CurrentItem.ID);

                targetSlot.AddItem(_itemSlot.CurrentItem, _itemSlot.Quantity);

                targetSlot.UI.RefreshUI();
                RemoveItem();

                return;
            }
        }
    }

    #endregion Double Click

    private void RemoveItem()
    {
        _itemSlot.SubtractItem(_itemSlot.Quantity);
        RefreshUI();
    }

    public void RefreshUI()
    {
        if (_itemSlot.CurrentItem != null)
        {
            _image.sprite = ItemSpriteDictionary.Instance.GetItemSprite(_itemSlot.CurrentItem.ID);
            UIManager.Instance.ChangeImageAlpha(_image, true);
        }
        else
        {
            _image.sprite = null;
            UIManager.Instance.ChangeImageAlpha(_image, false);
        }

        ChangeTexts();
    }

    private void OpenSlotMenu()
    {
        UIManager.Instance.OpenSlotMenu(transform.position, _isInventorySlotUI, _itemSlot.CurrentItem.Type);
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
            _countText.text = _itemSlot.Quantity.ToString();
    }

}
