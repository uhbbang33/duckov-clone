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
            _boxSlotsImage[index].gameObject.SetActive(false);
        }
        else
        {
            _boxSlotsImage[index].gameObject.SetActive(true);
            _boxSlotsImage[index].sprite = ItemSpriteDictionary.Instance.GetItemSprite(id);
        }
    }

}
