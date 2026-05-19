using UnityEngine;
using UnityEngine.InputSystem;

public class MapUIController : MonoBehaviour, IUICloseable
{
    [SerializeField] private GameObject _mapUI;
    [SerializeField] private GameObject _playerIcon;

    private GameManager _gameManager;
    private UIManager _uiManager;
    private PlayerMove _playerMove;

    private void Start()
    {
        _gameManager = GameManager.Instance;
        _uiManager = UIManager.Instance;
        _playerMove = _gameManager.PlayerObject.GetComponent<PlayerMove>();

        _gameManager.Actions.Player.Map.performed += OpenMap;
        _gameManager.Actions.Player.Cancel.performed += CloseMap;
    }

    private void OnDisable()
    {
        _gameManager.Actions.Player.Map.performed -= OpenMap;
        _gameManager.Actions.Player.Cancel.performed -= CloseMap;
    }

    private void OpenMap(InputAction.CallbackContext context)
    {
        IUICloseable ui = _uiManager.PeekStack();

        if (ui == null)
        {
            if (!_uiManager.TryPushStack(this))
                return;

            _mapUI.SetActive(true);
            SetPlayerIconTransform();
            _playerMove.StopMove();
        }
        else if (ui == (IUICloseable)this)
        {
            _uiManager.CloseTopUI();
        }
    }

    private void CloseMap(InputAction.CallbackContext context)
    {
        if (_uiManager.PeekStack() == (IUICloseable)this)
            _uiManager.CloseTopUI();
    }

    public void CloseUI()
    {
        _mapUI.SetActive(false);

        _playerMove.RestartMove();
    }

    private void SetPlayerIconTransform()
    {
        Vector3 playerPosition = _gameManager.PlayerObject.transform.position;

        _playerIcon.transform.position = new Vector3(playerPosition.x, _playerIcon.transform.position.y, playerPosition.z);
    }
}
