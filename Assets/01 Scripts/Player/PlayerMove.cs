using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    private InputActions _inputActions;
    private Rigidbody _rb;
    private Animator _anim;

    private Vector2 _moveDirection;
    private bool _isRun;

    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _runSpeed;

    private void Awake()
    {
        _inputActions = new InputActions();
        _rb = GetComponent<Rigidbody>();
        _anim = GetComponent<Animator>();

        _moveDirection = Vector2.zero;
        _isRun = false;
    }

    private void OnEnable()
    {
        _inputActions.Player.Enable();

        _inputActions.Player.Move.performed += OnMovePerformed;
        _inputActions.Player.Move.canceled += OnMoveCanceled;

        _inputActions.Player.Run.performed += OnRunPerformed;
        _inputActions.Player.Run.canceled += OnRunCanceled;
    }

    private void OnDisable()
    {
        _inputActions.Player.Move.performed -= OnMovePerformed;
        _inputActions.Player.Move.canceled -= OnMoveCanceled;

        _inputActions.Player.Run.performed -= OnRunPerformed;
        _inputActions.Player.Run.canceled -= OnRunCanceled;

        _inputActions.Player.Disable();
    }

    private void FixedUpdate()
    {
        float speed = _isRun ? _runSpeed : _walkSpeed;
        _rb.linearVelocity = new Vector3(_moveDirection.x * speed, 0f, _moveDirection.y * speed);
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        _moveDirection = context.ReadValue<Vector2>().normalized;
        _anim.SetBool("IsWalk", true);
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        _moveDirection = Vector2.zero;
        _anim.SetBool("IsWalk", false);
    }

    private void OnRunPerformed(InputAction.CallbackContext context)
    {
        _isRun = true;
        _anim.SetBool("IsRun", true);
    }
    private void OnRunCanceled(InputAction.CallbackContext context)
    {
        _isRun = false;
        _anim.SetBool("IsRun", false);
    }
}
