using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerClickHandler
{
    private ItemSlot _itemSlot;
    private Image _image;

    private float _lastClickTime;
    private const float _doubleClickThreshold = 0.25f;

    public ItemSlot Slot
    {
        get { return _itemSlot; }
        set { _itemSlot = value; }
    }

    private void Awake()
    {
        _image = GetComponent<Image>();
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

        ItemSlotUI targetSlot = eventData.pointerDrag.GetComponent<ItemSlotUI>();

        if (targetSlot == null || targetSlot == this)
            return;

        SwapItem(targetSlot);
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

        Refresh();
        target.Refresh();
    }

    #endregion Drag And Drop

   

    #region Double Click

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_itemSlot.CurrentItem == null)
            return;

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

        if (IsBoxSlot())
        {
            TryMoveToInventory();
        }
        else if (IsInventorySlot())
        {
            TryMoveToBox();
        }
    }

    private bool IsBoxSlot()
    {
        return transform.IsChildOf(GameManager.Instance.BoxRoot);
    }

    private bool IsInventorySlot()
    {
        return transform.IsChildOf(GameManager.Instance.InventoryRoot);
    }

    private void TryMoveToInventory()
    {
        if(GameManager.Instance.Inventory.TryAddItem(_itemSlot.CurrentItem, _itemSlot.Quantity))
        {
            RemoveItem();
        }
    }

    private void TryMoveToBox()
    {
        for (int i = 0; i < GameManager.Instance.BoxSlotNum; ++i)
        {
            ItemSlot targetSlot = GameManager.Instance.BoxItemSlots[i].GetComponent<ItemSlotUI>()._itemSlot;

            if (targetSlot.CurrentItem != null)
                continue;

            targetSlot.AddItem(_itemSlot.CurrentItem, _itemSlot.Quantity);

            targetSlot.UI.Refresh();

            RemoveItem();

            return;
        }
    }

    private void RemoveItem()
    {
        _itemSlot.SubtractItem(_itemSlot.Quantity);
        _image.sprite = null;
        UIManager.Instance.ChangeImageAlpha(_image, false);
    }

    #endregion Double Click

    private void Refresh()
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
    }
}
