using UnityEngine;
using UnityEngine.EventSystems;

public class TitleButtonHoverArrow : ButtonHoverSound, IPointerExitHandler
{
    [SerializeField] private GameObject _arrow;

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        _arrow.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _arrow.SetActive(false);
    }
}
