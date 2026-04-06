using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : SingletonMonoBehaviour<SceneLoader>
{
    [SerializeField] private float _loadDuration = 1.0f;

    private DataManager _dataManager;

    protected override void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        _dataManager = DataManager.Instance;
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

        _dataManager.SetDataForScene(sceneName);
        GameManager.Instance.CurrentSceneName = sceneName;

        StartCoroutine(LoadingUI.Instance.FadeOut());
    }
}
