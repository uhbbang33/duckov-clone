using UnityEngine;

public class CanvasLookAtCamera : MonoBehaviour
{
    private Camera _cam;

    private void OnEnable()
    {
        _cam = Camera.main;
    }

    private void LateUpdate()
    {
        transform.rotation = _cam.transform.rotation;
    }
}
