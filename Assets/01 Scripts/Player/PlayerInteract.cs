using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    // TODO : Temp
    [SerializeField] GameObject _boxUI;

    private InputActions _inputActions;
    private InteractableStateUI _ui;
    private PlayerInteractableScanner _scanner;
    private PlayerMove _playerMove;

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

        // TODO : OnInteract안으로
        if (_ui.Type == InteractableType.BOX)
        {
            _boxUI.SetActive(true);

            _scanner.HideAllInteractUI();

            _playerMove.StopMove();

            GameManager.Instance.CurrentBox.OpenBox();

            OnEnableInteractEvent?.Invoke();
        }

        _ui.OnInteract();
    }

    private void OnCancel(InputAction.CallbackContext context)
    {
        if (_ui == null) return;

        OnCloseUIEvent?.Invoke();

        UIManager.Instance.CloseSlotMenu();

        if (_ui.Type == InteractableType.BOX)
        {
            GameManager.Instance.CurrentOpenBox = null;

            _boxUI.SetActive(false);

            _scanner.StartCheck();

            _playerMove.RestartMove();

            OnDisableInteractEvent?.Invoke();
        }
    }
}
