using TMPro;
using UnityEngine;

public class StencilPlayerMask : StencilMask
{
    [SerializeField] private Transform _playerTransform;

    private void LateUpdate()
    {
        Vector3 targetPosition = new Vector3(_playerTransform.position.x, 0.3f, _playerTransform.position.z);
        transform.position = targetPosition;
    }
}
