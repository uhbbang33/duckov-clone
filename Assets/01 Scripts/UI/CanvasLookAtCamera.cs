using UnityEngine;

public class CanvasLookAtCamera : MonoBehaviour
{
    private Camera _cam;

    private void Awake()
    {
        _cam = Camera.main;
    }
    private void LateUpdate()
    {
        transform.forward = _cam.transform.forward;
    }
}
