using UnityEngine;
using UnityEngine.UI;

public class UIManager : SingletonMonoBehaviour<UIManager>
{
    [SerializeField] private Image[] _boxSlotsImage;
    [SerializeField] private GameObject _slotMenuUI;

    [SerializeField] private Vector3 _inventorySlotMenuOffset;
    [SerializeField] private Vector3 _boxSlotMenuOffset;

    [SerializeField] private GameObject _equipButton;
    [SerializeField] private GameObject _unloadButton;
    [SerializeField] private GameObject _useButton;
    [SerializeField] private GameObject _splitButton;
    [SerializeField] private GameObject _discardButton;


    public Image[] BoxSlots { get { return _boxSlotsImage; } }

    public void ChangeBoxSlotUI(int index, uint id)
    {
        if (id == 0)
        {
            _boxSlotsImage[index].sprite = null;
            ChangeImageAlpha(_boxSlotsImage[index], false);
        }
        else
        {
            _boxSlotsImage[index].sprite = ItemSpriteDictionary.Instance.GetItemSprite(id);
            ChangeImageAlpha(_boxSlotsImage[index], true);
        }
    }

    public void ChangeImageAlpha(Image image, bool showImage)
    {
        Color color = image.color;
        color.a = showImage ? 255f : 0f;
        image.color = color;
    }

    public void OpenSlotMenu(Vector3 pos, bool isInventorySlot, string itemType)
    {
        _slotMenuUI.transform.position = pos;

        if (isInventorySlot)
        {
            _slotMenuUI.transform.position += _inventorySlotMenuOffset;
        }
        else
        {
            _slotMenuUI.transform.position += _boxSlotMenuOffset;
        }

        if (IsUpperHalf(_slotMenuUI.transform.position))
        {
            // TODO
        }

        ShowButtonsByItemtype(itemType, isInventorySlot);

        _slotMenuUI.SetActive(true);
    }

    public void CloseSlotMenu()
    {
        _slotMenuUI.SetActive(false);
    }

    private bool IsUpperHalf(Vector3 pos)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(pos);
        return screenPos.y > Screen.height * 0.5f;
    }

    private void ShowButtonsByItemtype(string itemType, bool isInventorySlot)
    {
        // Temp
        _equipButton.SetActive(false);
        _unloadButton.SetActive(false);
        _useButton.SetActive(false);
        _splitButton.SetActive(false);
        _discardButton.SetActive(false);

        if (itemType == ItemType.Gun)
        {
            _equipButton.SetActive(true);
            _unloadButton.SetActive(true);
        }
        else if (itemType == ItemType.Medicine || itemType == ItemType.Food)
        {
            _useButton.SetActive(true);
            _splitButton.SetActive(true);
        }
        else if (itemType == ItemType.Ammo || itemType == ItemType.Etc)
        {
            _splitButton.SetActive(true);
        }

        if (isInventorySlot)
        {
            _discardButton.SetActive(true);
        }
    }
}
