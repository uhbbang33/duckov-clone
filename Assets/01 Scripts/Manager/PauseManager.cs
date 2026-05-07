using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class PauseManager : SingletonMonoBehaviour<PauseManager>
{
    [SerializeField] private GameObject _pauseUI;
    [SerializeField] private GameObject _pauseReturnToTitleUI;

    private GameManager _gameManager;
    private DataManager _dataManager;
    private InventoryController _inventoryController;

    private bool _isPaused = false;

    private void Start()
    {
        _gameManager = GameManager.Instance;
        _dataManager = DataManager.Instance;
        _inventoryController = _gameManager.PlayerObject.GetComponent<InventoryController>();

        _gameManager.Actions.UI.Pause.performed += OnPause;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        _gameManager.Actions.UI.Pause.performed -= OnPause;
    }

    private void OnPause(InputAction.CallbackContext context)
    {
        if (_inventoryController.InventoryIsOpen
            || _inventoryController.FrameClosedByCancel == Time.frameCount)
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

        _gameManager.Actions.Player.Disable();

        AudioListener.pause = true;
    }

    private void Resume()
    {
        UIManager.Instance.ShowCursor(false);
        Time.timeScale = 1f;

        _pauseUI.SetActive(false);
        _pauseReturnToTitleUI.SetActive(false);

        _gameManager.Actions.Player.Enable();

        AudioListener.pause = false;
    }

    #region On Button Clcik

    public void OnClickContinue()
    {
        _isPaused = false;
        Resume();
    }

    public void OnClickReturnToTitle()
    {
        _pauseReturnToTitleUI.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);
    }

    public void OnClickConfirmReturnToTitle()
    {
        _dataManager.SaveDataByScene();

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
