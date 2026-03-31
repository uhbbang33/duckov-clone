using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    private InputActions _inputActions;
    private InteractableStateUI _ui;
    private PlayerInteractableScanner _scanner;
    private PlayerMove _playerMove;

    private UIManager _uiManager;
    private GameManager _gameManager;

    public event Action OnEnableInteractEvent;
    public event Action OnDisableInteractEvent;
    public event Action OnCloseUIEvent;

    public InteractableStateUI UI
    {
        get { return _ui; }
        set { _ui = value; }
    }

    private void Awake()
    {
        _uiManager = UIManager.Instance;
        _gameManager = GameManager.Instance;
        _scanner = GetComponent<PlayerInteractableScanner>();
        _playerMove = GetComponent<PlayerMove>();
    }

    private void Start()
    {
        _inputActions = GetComponent<Player>().Actions;
        _inputActions.Player.Interact.performed += OnInteract;
        _inputActions.Player.Cancel.performed += OnCancel;
    }

    private void OnDisable()
    {
        _inputActions.Player.Interact.performed -= OnInteract;
        _inputActions.Player.Cancel.performed -= OnCancel;
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (_ui == null) return;

        _ui.HideCanvas();

        if (_ui.Type == InteractableType.BOX)
        {
            _uiManager.ShowBoxUI(true);
            _gameManager.CurrentBox.OpenBox();
        }
        else if(UI.Type == InteractableType.STORAGE)
        {
            _uiManager.ShowStorageUI(true);
        }

        _scanner.HideAllInteractUI();
        _playerMove.StopMove();
        OnEnableInteractEvent?.Invoke();

        _ui.OnInteract();
    }

    private void OnCancel(InputAction.CallbackContext context)
    {
        if (_ui == null) return;

        OnCloseUIEvent?.Invoke();

        _uiManager.CloseSlotMenu();

        if (_ui.Type == InteractableType.BOX)
        {
            _gameManager.CurrentOpenBox = null;
            _uiManager.ShowBoxUI(false);
        }
        else if (UI.Type == InteractableType.STORAGE)
        {
            _uiManager.ShowStorageUI(false);
        }

        _scanner.StartCheck();
        _playerMove.RestartMove();
        OnDisableInteractEvent?.Invoke();
    }
}
