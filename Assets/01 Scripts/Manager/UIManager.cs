using UnityEngine;
using UnityEngine.UI;

public class UIManager : SingletonMonoBehaviour<UIManager>
{
    [SerializeField] private Image[] _boxSlotsImage;
    [SerializeField] private GameObject _slotMenuUI;

    [SerializeField] private Vector3 _inventorySlotMenuOffset;
    [SerializeField] private Vector3 _boxSlotMenuOffset;

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

    public void OpenSlotMenu(Vector3 pos, bool isInventorySlot)
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
            //_slotMenuUI.transform.position += 
        }

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
}
