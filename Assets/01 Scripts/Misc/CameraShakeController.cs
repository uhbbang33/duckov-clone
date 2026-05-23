using Unity.Cinemachine;
using Unity.Mathematics;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

public class CameraShakeController : MonoBehaviour
{
    [SerializeField] private CinemachineImpulseSource _impulseSource;
    [SerializeField] private float _speedMultiplier = 0.001f;
    [SerializeField] private float _impulseDuration = 0.1f;

    public void ShakeOnFire(Vector3 fireDir, float speed)
    {
        float force = speed * _speedMultiplier;

        Quaternion cameraRot = Quaternion.Euler(-45f, 45f, 0f);
        Vector3 shakeDir = (cameraRot * fireDir).normalized;

        _impulseSource.ImpulseDefinition.ImpulseDuration = _impulseDuration;
        _impulseSource.GenerateImpulseWithVelocity(shakeDir * force);
    }
}
