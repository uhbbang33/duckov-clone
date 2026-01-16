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

    public event Action<bool> OnInteractEvent;

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

        _boxUI.SetActive(true);

        _scanner.HideAllInteractUI();

        _playerMove.StopMove();

        GameManager.Instance.CurrentBox.OpenBox();

        OnInteractEvent?.Invoke(true);
    }

    private void OnCancel(InputAction.CallbackContext context)
    {
        _boxUI.SetActive(false);

        _scanner.StartCheck();

        _playerMove.RestartMove();

        OnInteractEvent?.Invoke(false);
    }
}
