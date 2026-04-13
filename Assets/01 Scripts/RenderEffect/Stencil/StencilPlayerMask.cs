using UnityEngine;

public class StencilPlayerMask : StencilMask
{
    [SerializeField] private Transform _playerTransform;

    private void LateUpdate()
    {
        Vector3 dir = (Camera.main.transform.position - _playerTransform.position).normalized;

        transform.position = _playerTransform.position + dir * _distanceFromCamera;
    }
}
