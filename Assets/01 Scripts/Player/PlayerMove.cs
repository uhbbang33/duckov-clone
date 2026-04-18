using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    private GameManager _gameManager;
    private Player _player;
    private InputActions _inputActions;
    private Rigidbody _rb;
    private Animator _anim;
    private StaminaPoint _sp;
    private Hydration _hydration;
    private Transform _lookBaseTransform;
    private PlayerShooting _playerShooting;

    private Vector2 _moveInput;
    private Vector2 _mousePosition;
    private Vector3 _lookDirection;
    private Vector3 _rollDirection;
    private float _rollCoolTime;
    private bool _isRunButtonPressed;
    private float _speedDebuffRate;
    private bool _isZeroHydration;

    private PlayerMoveData _playerMoveData;

    [SerializeField] private float _mouseTurnSpeed;
    [SerializeField] private float _runTurnSpeed;
    [SerializeField] private Transform _originLookBaseTransform;

    private WaitForSeconds _waitForRoll;

    public event Action OnRun;
    public event Action OnRunCancel;
    public event Action OnWalk;
    public event Action OnWalkCancel;

    public Transform LookBaseTransform
    {
        get { return _lookBaseTransform; }
        set
        {
            _lookBaseTransform = value;

            if (value == null)
            {
                _lookBaseTransform = _originLookBaseTransform;
            }
        }
    }

    #region MonoBehaviour

    private void Awake()
    {
        _player = GetComponent<Player>();
        _player.OnPlayerDataInitialized += MoveDataSetup;

        _rb = GetComponent<Rigidbody>();
        _anim = GetComponent<Animator>();
        _sp = GetComponent<StaminaPoint>();
        _sp.OnSPZero += StopRun;

        _hydration = GetComponent<Hydration>();
        _hydration.OnEnterZeroHydration += EnableZeroHydration;
        _hydration.OnExitZeroHydration += DisableZeroHydration;

        _playerShooting = GetComponent<PlayerShooting>();

        _moveInput = Vector2.zero;
        _mousePosition = Vector2.zero;
        _lookDirection = Vector3.zero;
        _rollDirection = Vector3.zero;
        
        _lookBaseTransform = _originLookBaseTransform;
    }

    private void Start()
    {
        _gameManager = GameManager.Instance;
        _inputActions = _gameManager.Actions;

        SubscribeInputActions();
    }

    private void OnDisable()
    {
        UnsubscribeInputActions();

        _sp.OnSPZero -= StopRun;
        _hydration.OnEnterZeroHydration -= EnableZeroHydration;
        _hydration.OnExitZeroHydration -= DisableZeroHydration;
    }

    private void FixedUpdate()
    {
        Vector3 dir = new();
        float speed = 0f;

        if (_player.State == PlayerState.Rolling)
        {
            dir = _rollDirection;
            speed = _playerMoveData.RollMoveSpeed;
        }
        else
        {
            dir = SetDirection(_moveInput);
            speed = (_player.State == PlayerState.Running) ? _playerMoveData.RunSpeed : _playerMoveData.WalkSpeed;
        }

        speed *= ((100f - _speedDebuffRate) / 100f);

        if (_isZeroHydration)
            speed /= 2;

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


    private void MoveDataSetup()
    {
        _playerMoveData = _player.MoveData;
        _waitForRoll = new WaitForSeconds(_playerMoveData.RollDuration);
    }

    private void LookAtMouse(Vector2 mousePos)
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            Vector3 dir = hit.point - _lookBaseTransform.position;

            dir.y = 0;

            if (dir.sqrMagnitude > 0.01f
                && _player.State != PlayerState.Rolling)
            {
                Vector3 lookDir = dir.normalized;
                _lookDirection = lookDir;

                Quaternion targetRotation;
                if (_player.State == PlayerState.Running)
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
        StopRun();
        _anim.SetBool("IsWalk", false);
        _moveInput = Vector2.zero;

        OnWalkCancel?.Invoke();

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

        if (_isRunButtonPressed)
        {
            StartRun();
        }
        else
        {
            OnWalk?.Invoke();
            _player.ChangePlayerState(PlayerState.Walking);
        }
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        _moveInput = Vector2.zero;
        _anim.SetBool("IsWalk", false);

        _sp.IsReducing = false;

        if (_player.State != PlayerState.Rolling)
            _player.ChangePlayerState(PlayerState.Idle);

        OnWalkCancel?.Invoke();
    }

    private void OnRunPerformed(InputAction.CallbackContext context)
    {
        _isRunButtonPressed = true;
        StartRun();
    }

    private void OnRunCanceled(InputAction.CallbackContext context)
    {
        _isRunButtonPressed = false;
        StopRun();
    }

    private void OnRollPerformed(InputAction.CallbackContext context)
    {
        if (_player.State == PlayerState.Rolling || _rollCoolTime > 0f)
            return;

        if (_sp.CurrentSP < _playerMoveData.RollSPCost) 
            return;

        _sp.ReduceSPImmediately(_playerMoveData.RollSPCost);

        _player.ChangePlayerState(PlayerState.Rolling);

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

    private void StartRun()
    {
        if (_sp.CurrentSP < _playerMoveData.RunSPCost)
            return;

        if (_rb.linearVelocity == Vector3.zero)
            return;

        if (_gameManager.CurrentSceneName != SceneName.BunkerScene)
            _sp.ReduceSPPerSecond(_playerMoveData.RunSPCost);

        _player.ChangePlayerState(PlayerState.Running);
        _anim.SetBool("IsRun", true);
        _playerShooting.IsFirePressed = false;
        OnRun?.Invoke();
    }

    private void StopRun()
    {
        if (_player.State != PlayerState.Running)
            return;

        if (_moveInput != Vector2.zero)
            _player.ChangePlayerState(PlayerState.Walking);
        else
            _player.ChangePlayerState(PlayerState.Idle);

        _anim.SetBool("IsRun", false);
        _sp.IsReducing = false;
        OnRunCancel?.Invoke();
    }

    private void EnableZeroHydration()
    {
        _isZeroHydration = true;
    }

    private void DisableZeroHydration()
    {
        _isZeroHydration = false;
    }

    public void ChangeSpeed(float ReducePercentage)
    {
        if (ReducePercentage >= 25f && ReducePercentage < 50f)
            _speedDebuffRate = 10f;
        else if (ReducePercentage >= 50f && ReducePercentage < 75f)
            _speedDebuffRate = 20f;
        else if (ReducePercentage >= 75f && ReducePercentage < 100f)
            _speedDebuffRate = 40f;
        else if (ReducePercentage >= 100f)
            _speedDebuffRate = 100f;
        else
            _speedDebuffRate = 0f;
    }


    private IEnumerator RollRoutine()
    {
        yield return _waitForRoll;

        _rollCoolTime = _playerMoveData.RollCooldown;

        if (_moveInput != Vector2.zero)
        {
            if (_isRunButtonPressed)
                _player.ChangePlayerState(PlayerState.Running);
            else
                _player.ChangePlayerState(PlayerState.Walking);
        }
        else
            _player.ChangePlayerState(PlayerState.Idle);
    }
}
