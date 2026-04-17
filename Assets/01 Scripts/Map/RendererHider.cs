using UnityEngine;

public class RendererHider : MonoBehaviour
{
    [SerializeField] private Renderer[] _hideTargetRenderer;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tag.Player))
        {
            foreach (Renderer renderer in _hideTargetRenderer)
            {
                renderer.enabled = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Tag.Player))
        {
            foreach (Renderer renderer in _hideTargetRenderer)
            {
                renderer.enabled = true;
            }
        }
    }
}
