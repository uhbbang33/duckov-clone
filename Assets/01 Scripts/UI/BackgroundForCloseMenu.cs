using UnityEngine;
using UnityEngine.EventSystems;

public class BackgroundForCloseMenu : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        UIManager.Instance.CloseSlotMenu();
    }
}