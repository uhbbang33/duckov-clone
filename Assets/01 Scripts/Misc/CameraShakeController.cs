using Unity.Cinemachine;
using UnityEngine;

public class CameraShakeController : MonoBehaviour
{
    [SerializeField] private CinemachineImpulseSource _impulseSource;
    [SerializeField] private float _speedMultiplier = 0.001f;
    [SerializeField] private float _impulseDuration = 0.1f;

    public void ShakeOnFire(Vector3 fireDir, float speed)
    {
        float force = speed * _speedMultiplier;

        Transform camTransform = Camera.main.transform;
        float rightComponent = Vector3.Dot(fireDir, camTransform.right);
        float upComponent = Vector3.Dot(fireDir, camTransform.up);

        Vector3 shakeDir = (camTransform.right * rightComponent + camTransform.up * upComponent).normalized;

        _impulseSource.ImpulseDefinition.ImpulseDuration = _impulseDuration;
        _impulseSource.GenerateImpulseWithVelocity(shakeDir * force);
    }
}
