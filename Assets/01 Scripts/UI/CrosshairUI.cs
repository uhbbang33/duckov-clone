using UnityEngine;
using UnityEngine.InputSystem;

public class CrosshairUI : MonoBehaviour
{
    private RectTransform _rect;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
        ShowCursor(false);
    }

    private void Update()
    {
        _rect.position = Mouse.current.position.ReadValue();
    }

    public void ShowCursor(bool show)
    {
        Cursor.visible = show;
    }

}
