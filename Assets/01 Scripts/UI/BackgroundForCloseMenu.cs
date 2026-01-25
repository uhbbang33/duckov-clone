using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BackgroundForCloseMenu : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private LayerMask _slotUILayer;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right)
        {
            UIManager.Instance.CloseSlotMenu();
            return;
        }

        List<RaycastResult> results = new();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results)
        {
            ItemSlotUI slotUI = result.gameObject.GetComponent<ItemSlotUI>();
            if (slotUI == null)
                continue;

            slotUI.OpenSlotMenu();
            return;
        }

        UIManager.Instance.CloseSlotMenu();
    }
}