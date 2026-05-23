using Unity.Cinemachine;
using UnityEngine;

public class CameraShakeController : MonoBehaviour
{
    [SerializeField] private CinemachineImpulseSource _impulseSource;
    [SerializeField] private float _speedMultiplier = 0.001f;
    [SerializeField] private float _impulseDuration = 0.1f;

    public void ShakeOnFire(Vector3 shakeDirection, float speed)
    {
        float force = speed * _speedMultiplier;
        
        _impulseSource.ImpulseDefinition.ImpulseDuration = _impulseDuration;

        _impulseSource.GenerateImpulseWithVelocity(shakeDirection * force);
    }
}
