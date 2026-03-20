using UnityEngine;
using UnityEngine.EventSystems;

public class TitleButtonHoverArrow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject _arrow;

    public void OnPointerEnter(PointerEventData eventData)
    {
        _arrow.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _arrow.SetActive(false);
    }
}
