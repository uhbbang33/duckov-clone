using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : SingletonMonoBehaviour<SceneLoader>
{
    private const float _loadDuration = 1.0f;


    protected override void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadRoutine(sceneName));
    }

    private IEnumerator LoadRoutine(string sceneName)
    {
        yield return LoadingUI.Instance.FadeIn();


        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        while (op.progress < 0.9f)
            yield return null;

        yield return new WaitForSeconds(_loadDuration);

        op.allowSceneActivation = true;

        while (!op.isDone)
            yield return null;


        yield return LoadingUI.Instance.FadeOut();
    }
}
