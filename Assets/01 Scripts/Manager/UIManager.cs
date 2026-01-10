using UnityEngine;
using UnityEngine.UI;

public class UIManager : SingletonMonoBehaviour<UIManager>
{
    [SerializeField] private Image[] _boxSlotsImage;

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

}
