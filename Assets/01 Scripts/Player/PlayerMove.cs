using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    private InputActions _inputActions;
    private Rigidbody _rb;
    private Animator _anim;
    private StaminaPoint _sp;

    private Vector2 _moveInput;
    private Vector2 _mousePosition;
    private Vector3 _lookDirection;
    private Vector3 _rollDirection;
    private float _rollCoolTime;
    private bool _isRun;
    private bool _isRoll;

    [SerializeField] private float _rollTickSPCost;
    [SerializeField] private float _runTickSPCost;
    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _runSpeed;
    [SerializeField] private float _rollSpeed;
    [SerializeField] private float _rollTime;
    [SerializeField] private float _rollMaxCoolTime;
    [SerializeField] private float _mouseTurnSpeed;
    [SerializeField] private float _runTurnSpeed;

    private WaitForSeconds _waitForRoll;

    #region MonoBehaviour

    private void Awake()
    {
        Initialize();
    }

    private void Start()
    {
        _inputActions = GetComponent<Player>().Actions;
        SubscribeInputActions();

        _sp.OnSPZero += StopRun;
    }

    private void OnDisable()
    {
        UnsubscribeInputActions();

        _sp.OnSPZero -= StopRun;
    }

    private void FixedUpdate()
    {
        Vector3 dir = new();
        float speed = 0f;

        if (_isRoll)
        {
            dir = _rollDirection;
            speed = _rollSpeed;
        }
        else
        {
            dir = SetDirection(_moveInput);
            speed = _isRun ? _runSpeed : _walkSpeed;
        }

        if (dir.sqrMagnitude > 0.01f)
            _rb.linearVelocity = new Vector3(dir.x * speed, _rb.linearVelocity.y, dir.z * speed);
        else
            _rb.linearVelocity = Vector3.zero;
    }

    private void Update()
    {
        if (_rollCoolTime > 0f)
            _rollCoolTime -= Time.deltaTime;

        LookAtMouse(_mousePosition);
    }

    #endregion MonoBehaviour


    private void Initialize()
    {
        _rb = GetComponent<Rigidbody>();
        _anim = GetComponent<Animator>();
        _sp = GetComponent<StaminaPoint>();

        _moveInput = Vector2.zero;
        _mousePosition = Vector2.zero;
        _lookDirection = Vector3.zero;
        _rollDirection = Vector3.zero;
        _isRun = false;
        _isRoll = false;

        _waitForRoll = new WaitForSeconds(_rollTime);
    }

    private void LookAtMouse(Vector2 mousePos)
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            Vector3 dir = hit.point - transform.position;
            dir.y = 0f;

            if (dir.sqrMagnitude > 0.01f && !_isRoll)
            {
                Vector3 lookDir = dir.normalized;
                _lookDirection = lookDir;

                if (!_isRoll)
                {
                    Quaternion targetRotation;
                    if (_isRun)
                    {
                        Vector3 runDir = SetDirection(_moveInput);
                        if (runDir.sqrMagnitude > 0.01f)
                        {
                            targetRotation = Quaternion.LookRotation(runDir);
                            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, _runTurnSpeed * Time.deltaTime);
                        }
                    }
                    else
                    {
                        targetRotation = Quaternion.LookRotation(lookDir);
                        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, _mouseTurnSpeed * Time.deltaTime);
                    }
                }
            }
        }
    }

    private Vector3 SetDirection(Vector2 input)
    {
        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        return camForward * input.y + camRight * input.x;
    }

    public void StopMove()
    {
        _anim.SetBool("IsWalk", false);
        _moveInput = Vector2.zero;

        UnsubscribeInputActions();
    }

    public void RestartMove()
    {
        SubscribeInputActions();
    }

    #region Input System

    private void SubscribeInputActions()
    {
        _inputActions.Player.Move.performed += OnMovePerformed;
        _inputActions.Player.Move.canceled += OnMoveCanceled;

        _inputActions.Player.Run.performed += OnRunPerformed;
        _inputActions.Player.Run.canceled += OnRunCanceled;

        _inputActions.Player.Roll.performed += OnRollPerformed;

        _inputActions.Player.Look.performed += OnLook;
    }

    private void UnsubscribeInputActions()
    {
        _inputActions.Player.Move.performed -= OnMovePerformed;
        _inputActions.Player.Move.canceled -= OnMoveCanceled;

        _inputActions.Player.Run.performed -= OnRunPerformed;
        _inputActions.Player.Run.canceled -= OnRunCanceled;

        _inputActions.Player.Roll.performed -= OnRollPerformed;

        _inputActions.Player.Look.performed -= OnLook;
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>().normalized;
        _anim.SetBool("IsWalk", true);
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        _moveInput = Vector2.zero;
        _anim.SetBool("IsWalk", false);
    }

    private void OnRunPerformed(InputAction.CallbackContext context)
    {
        if (_sp.CurrentSP < _runTickSPCost)
            return;

        _sp.ReduceSPPerSecond(_runTickSPCost);
        
        _isRun = true;
        _anim.SetBool("IsRun", true);
    }

    private void OnRunCanceled(InputAction.CallbackContext context)
    {
        StopRun();
    }

    private void OnRollPerformed(InputAction.CallbackContext context)
    {
        if (_isRoll || _rollCoolTime > 0f)
            return;

        if (_sp.CurrentSP < _rollTickSPCost) 
            return;

        _sp.ReduceSPImmediately(_rollTickSPCost);

        _isRoll = true;

        // dir - move
        if (_moveInput != Vector2.zero)
        {
            Vector3 dir = SetDirection(_moveInput);
            if (dir.sqrMagnitude > 0.01f)
            {
                transform.rotation = Quaternion.LookRotation(dir);
            }
            _rollDirection = dir;
        }
        else // dir - mouse
        {
            _rollDirection = _lookDirection;
        }

        StartCoroutine(RollRoutine());
        
        _anim.SetTrigger("Roll");
    }

    private void OnLook(InputAction.CallbackContext context)
    {
        _mousePosition = context.ReadValue<Vector2>();
    }

    #endregion Input System

    private void StopRun()
    {
        _sp.IsReducing = false;
        _isRun = false;
        _anim.SetBool("IsRun", false);
    }


    private IEnumerator RollRoutine()
    {
        yield return _waitForRoll;

        _rollCoolTime = _rollMaxCoolTime;

        _isRoll = false;
    }
}
