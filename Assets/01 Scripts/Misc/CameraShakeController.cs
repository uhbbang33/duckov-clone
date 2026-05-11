using Unity.Cinemachine;
using UnityEngine;

public class CameraShakeController : MonoBehaviour
{
    [SerializeField] private CinemachineImpulseSource _impulseSource;
    [SerializeField] private float _baseImpulseForce;

    public void ShakeOnFire(Vector3 shakeDirection, float speed)
    {
        float force = _baseImpulseForce + speed * 0.005f;
        _impulseSource.GenerateImpulseWithVelocity(-shakeDirection * force);
    }
}
