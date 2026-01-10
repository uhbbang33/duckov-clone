using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    private ItemSlot _itemSlot;
    private Image _image;

    public ItemSlot Slot
    {
        get { return _itemSlot; }
        set { _itemSlot = value; }
    }

    private void Awake()
    {
        _image = GetComponent<Image>();
    }

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
        {
            if (targetSlot == null)
                Debug.Log("Target Slot null");
            else if(targetSlot == this)
                Debug.Log("Target Slot this");

            return;
        }

        SwapItem(targetSlot);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.localPosition = Vector3.zero;
        _image.raycastTarget = true;
    }

    private void SwapItem(ItemSlotUI target)
    {
        if (target._itemSlot.CurrentItem != null)
        {
            Debug.Log("Target : " + target._itemSlot.CurrentItem.Name);
            Debug.Log("Target : " + target._itemSlot.Quantity);
        }
        else
            Debug.Log("Target is Null");

        if (_itemSlot.CurrentItem != null)
        {
            Debug.Log("origin : " + _itemSlot.CurrentItem.Name);
            Debug.Log("origin : " + _itemSlot.Quantity);
        }
        else
            Debug.Log("Origin is Null");


        (target._itemSlot.CurrentItem, _itemSlot.CurrentItem) = (_itemSlot.CurrentItem, target._itemSlot.CurrentItem);

        (target._itemSlot.Quantity, _itemSlot.Quantity) = (_itemSlot.Quantity, target._itemSlot.Quantity);

        Refresh();
        target.Refresh();
    }

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
