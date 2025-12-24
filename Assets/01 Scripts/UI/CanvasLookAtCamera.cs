using UnityEngine;

public class CanvasLookAtCamera : MonoBehaviour
{
    private void LateUpdate()
    {
        Camera cam = Camera.main;

        transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward,
            cam.transform.rotation * Vector3.up);
    }
}
