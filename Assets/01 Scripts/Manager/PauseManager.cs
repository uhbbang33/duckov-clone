using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class PauseManager : SingletonMonoBehaviour<PauseManager>
{
    [SerializeField] private GameObject _pauseUI;
    [SerializeField] private GameObject _pauseReturnToTitleUI;

    private GameManager _gameManager;
    private SaveManager _saveManager;

    private bool _isPaused = false;

    private void Start()
    {
        _gameManager = GameManager.Instance;
        _saveManager = SaveManager.Instance;

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

        EventSystem.current.SetSelectedGameObject(null);

        // TODO : Sound Pause
    }

    private void Resume()
    {
        UIManager.Instance.ShowCursor(false);
        Time.timeScale = 1f;

        _pauseUI.SetActive(false);
        _pauseReturnToTitleUI.SetActive(false);
    }

    #region On Button Clcik

    public void OnClickContinue()
    {
        _isPaused = false;
        Resume();
    }

    public void OnClickReturnToTile()
    {
        _pauseReturnToTitleUI.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);
    }

    public void OnClickConfirmReturnToTitle()
    {
        _isPaused = false;
        Resume();

        _saveManager.SavePlayerStats();
        _saveManager.SavePlayerInventory();
        _saveManager.SaveStorage();

        SceneLoader.Instance.LoadScene(SceneName.TitleScene);
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
