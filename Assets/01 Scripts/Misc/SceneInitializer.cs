using System;
using System.Collections;
using UnityEngine;

// 씬에서 가장 늦게
[DefaultExecutionOrder(1000)]
public class SceneInitializer : MonoBehaviour
{
    public static event Action OnSceneReady;

    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame(); // 모든 start 실행 대기
        yield return null;
        OnSceneReady?.Invoke();
        OnSceneReady = null;
    }
}