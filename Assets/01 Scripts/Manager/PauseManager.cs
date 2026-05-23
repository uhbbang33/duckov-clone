using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class PauseManager : SingletonMonoBehaviour<PauseManager>, IUICloseable
{
    [SerializeField] private GameObject _pauseUI;
    [SerializeField] private GameObject _pauseReturnToTitleUI;

    private GameManager _gameManager;
    private DataManager _dataManager;
    private UIManager _uiManager;

    private void Start()
    {
        _gameManager = GameManager.Instance;
        _dataManager = DataManager.Instance;
        _uiManager = UIManager.Instance;

        _gameManager.Actions.UI.Pause.performed += OnPause;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        _gameManager.Actions.UI.Pause.performed -= OnPause;
    }

    private void OnPause(InputAction.CallbackContext context)
    {
        IUICloseable peekObject = _uiManager.PeekStack();

        if (peekObject == null)
            Pause();
        else if (peekObject == (IUICloseable)this)
            _uiManager.CloseTopUI();
    }

    public void CloseUI()
    {
        Resume();
    }

    private void Pause()
    {
        if (!_uiManager.TryPushStack(this))
            return;

        _uiManager.ShowCursor(true);

        Time.timeScale = 0f;

        _pauseUI.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);

        _gameManager.Actions.Player.Disable();

        AudioListener.pause = true;
    }

    private void Resume()
    {
        _uiManager.ShowCursor(false);

        Time.timeScale = 1f;

        _pauseUI.SetActive(false);
        _pauseReturnToTitleUI.SetActive(false);

        _gameManager.Actions.Player.Enable();

        AudioListener.pause = false;
    }

    #region On Button Clcik

    public void OnClickContinue()
    {
        _uiManager.CloseTopUI();
    }

    public void OnClickReturnToTitle()
    {
        _pauseReturnToTitleUI.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);
    }

    public void OnClickConfirmReturnToTitle()
    {
        _dataManager.SaveDataByScene(SceneName.TitleScene);

        _gameManager.Actions.Player.Cancel.performed -= OnPause;

        SceneLoader.Instance.LoadScene(SceneName.TitleScene);
    }

    public void OnClickSettings()
    {

    }

    public void OnClickQuit()
    {
        _gameManager.QuitGame();
    }

   public void OnClickCancel()
    {
        _pauseReturnToTitleUI.SetActive(false);
    }

    #endregion On Button Clcik
}
