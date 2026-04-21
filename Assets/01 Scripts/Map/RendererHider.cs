using UnityEngine;

public class RendererHider : MonoBehaviour
{
    [SerializeField] private Renderer[] _hideTargetRenderer;

    private bool _isPlayerInside;
    private bool _isMouseHovering;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tag.Player))
        {
            _isPlayerInside = true;
            UpdateRoofVisibility();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Tag.Player))
        {
            _isPlayerInside = false;
            UpdateRoofVisibility();
        }
    }

    public void MouseHover(bool isHovering)
    {
        if (_isMouseHovering == isHovering)
            return;

        _isMouseHovering = isHovering;
        UpdateRoofVisibility();
    }

    private void UpdateRoofVisibility()
    {
        bool isShow = _isPlayerInside || _isMouseHovering;

        foreach (Renderer renderer in _hideTargetRenderer)
        {
            renderer.enabled = !isShow;
        }
    }
}
