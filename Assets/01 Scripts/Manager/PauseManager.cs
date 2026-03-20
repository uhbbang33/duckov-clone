using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseManager : SingletonMonoBehaviour<PauseManager>
{
    [SerializeField] private GameObject _pauseUI;

    private GameManager _gameManager;

    private bool _isPaused = false;

    private void Start()
    {
        _gameManager = GameManager.Instance;

        _gameManager.Actions.Player.Cancel.performed += OnPause;
    }

    private void OnDestroy()
    {
        _gameManager.Actions.Player.Cancel.performed -= OnPause;
    }

    private void OnPause(InputAction.CallbackContext context)
    {
        if (_gameManager.Inventory.InventoryIsOpen)
            return;

        _isPaused = !_isPaused;

        if (_isPaused)
            Pause();
        else
            Resume();
    }

    private void Pause()
    {
        UIManager.Instance.ShowCursor(true);
        Time.timeScale = 0f;

        _pauseUI.SetActive(true);

        // TODO : Sound Pause
    }

    private void Resume()
    {
        UIManager.Instance.ShowCursor(false);
        Time.timeScale = 1f;

        _pauseUI.SetActive(false);
    }

    #region On Button Clcik

    public void OnClickContinue()
    {
        _isPaused = false;
        Resume();
    }

    public void OnClickTitle()
    {
        _isPaused = false;
        Resume();

        SceneManager.LoadSceneAsync(SceneName.TitleScene);
    }

    public void OnClickSettings()
    {

    }

    public void OnClickQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    #endregion On Button Clcik
}
