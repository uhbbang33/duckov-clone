using Unity.Cinemachine;
using UnityEngine;

public class CameraShakeController : MonoBehaviour
{
    [SerializeField] private CinemachineImpulseSource _impulseSource;
    [SerializeField] private float _baseImpulseForce;
    [SerializeField] private float _speedMultiplier = 0.001f;

    public void ShakeOnFire(Vector3 shakeDirection, float speed)
    {
        float force = _baseImpulseForce + speed * _speedMultiplier;
        _impulseSource.GenerateImpulseWithVelocity(shakeDirection * force);
    }
}
