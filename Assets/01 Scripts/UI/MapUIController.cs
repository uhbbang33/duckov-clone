using UnityEngine;
using UnityEngine.InputSystem;

public class MapUIController : MonoBehaviour, IUICloseable
{
    [SerializeField] private GameObject _mapUI;

    private GameManager _gameManager;
    private UIManager _uiManager;

    private void Start()
    {
        _gameManager.Actions.Player.Map.performed += OpenMap;
        _gameManager.Actions.Player.Cancel.performed += CloseMap;

        _uiManager = UIManager.Instance;
    }

    private void OnDisable()
    {
        _gameManager.Actions.Player.Map.performed -= OpenMap;
        _gameManager.Actions.Player.Cancel.performed -= CloseMap;
    }

    private void OpenMap(InputAction.CallbackContext context)
    {
        if (_uiManager.PeekStack() == null)
        {
            _mapUI.SetActive(true);
            _uiManager.PushStack(this);
        }
    }
    private void CloseMap(InputAction.CallbackContext context)
    {
        _uiManager.CloseTopUI();
    }

    public void CloseUI()
    {
        _mapUI.SetActive(false);
    }
}
