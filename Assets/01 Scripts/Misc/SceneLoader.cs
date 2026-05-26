using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : SingletonMonoBehaviour<SceneLoader>
{
    [SerializeField] private LoadingUI _loadingUI;
    [SerializeField] private float _loadDuration = 1.0f;

    private DataManager _dataManager;
    private GameManager _gameManager;
    private Coroutine _coroutine;

    protected override void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        _dataManager = DataManager.Instance;
        _gameManager = GameManager.Instance;
    }

    public void LoadScene(string sceneName)
    {
        if (_coroutine != null)
            return;

        _coroutine = StartCoroutine(LoadRoutine(sceneName));
    }

    private IEnumerator LoadRoutine(string sceneName)
    {
        _gameManager.DisableInputActions();

        yield return _loadingUI.FadeIn();

        _gameManager.Inventory.ClearInventory();

        bool isSceneReady = false;
        SceneInitializer.OnSceneReady += () => isSceneReady = true;

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        while (op.progress < 0.9f)
            yield return null;

        Time.timeScale = 1f;
        yield return new WaitForSeconds(_loadDuration);

        op.allowSceneActivation = true;

        GameManager.Instance.CurrentSceneName = sceneName;

        while (!op.isDone)
            yield return null;

        yield return new WaitUntil(() => isSceneReady);

        _dataManager.SetDataByScene(sceneName);

        StartCoroutine(_loadingUI.FadeOut());

        _gameManager.EnableInputActions();
        _coroutine = null;

        Time.timeScale = 1f;
        AudioListener.pause = false;
    }
}
