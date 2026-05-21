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
    private BunkerManager _bunkerManager;
    private FieldManager _fieldManager;

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
        _scanner = GetComponent<PlayerInteractableScanner>();
        _playerMove = GetComponent<PlayerMove>();
    }

    private void Start()
    {
        _uiManager = UIManager.Instance;
        _bunkerManager = BunkerManager.Instance;
        _fieldManager = FieldManager.Instance;
        _inputActions = GameManager.Instance.Actions;
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
            _fieldManager.CurrentBox.OpenBox();
        }
        else if (_ui.Type == InteractableType.STORAGE)
        {
            _bunkerManager.IsStorageOpened = true;
            _uiManager.ShowStorageUI(true);
        }
        else if (_ui.Type == InteractableType.SHOP)
        {
            _bunkerManager.IsShopOpened = true;
            _uiManager.ShowShopUI(true);
        }
        else if(_ui.Type == InteractableType.DROPPEDITEM)
        {
            _ui.OnInteract();
            return;
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
            _fieldManager.CurrentOpenBox = null;
            _uiManager.ShowBoxUI(false);
        }
        else if (_ui.Type == InteractableType.STORAGE)
        {
            _bunkerManager.IsStorageOpened = false;
            _uiManager.ShowStorageUI(false);
        }
        else if (_ui.Type == InteractableType.SHOP)
        {
            _bunkerManager.IsShopOpened = false;
            _uiManager.ShowShopUI(false);
        }

        _scanner.StartCheck();
        _playerMove.RestartMove();
        OnDisableInteractEvent?.Invoke();
    }
}
