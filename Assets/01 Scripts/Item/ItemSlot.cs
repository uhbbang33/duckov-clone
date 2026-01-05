using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    [SerializeField] private GameObject _slotUI;

    private Item _currentItem;
    private Image _currentImage;
    
    private void Awake()
    {
        _currentImage = _slotUI.GetComponent<Image>();
    }

    public void SetItem(uint id)
    {
        _slotUI.SetActive(true);

        //_currentItem = GetItem(id);
        SetSprite(id);
    }

    private void SetSprite(uint id)
    {
        _currentImage.sprite = ItemSpriteDictionary.Instance.GetItemSprite(id);
    }

    public void RemoveItem()
    {
        RemoveSprite();
        _currentItem = null;

        _slotUI.SetActive(false);
    }

    private void RemoveSprite()
    {
        _currentImage.sprite = null;
    }
}
