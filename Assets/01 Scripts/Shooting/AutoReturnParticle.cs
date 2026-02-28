using System.Collections;
using UnityEngine;

public class AutoReturnParticle : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particle;

    private void OnEnable()
    {
        StartCoroutine(ReturnPoolRoutine());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    IEnumerator ReturnPoolRoutine()
    {
        yield return null; // 파티클이 시작할때까지 최소 1프레임 대기

        yield return new WaitUntil(() => !_particle.IsAlive(true));

        PoolManager.Instance.ReturnObject(PoolId.MuzzleFlash, gameObject);
    }
}
