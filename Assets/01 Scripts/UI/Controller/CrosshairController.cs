using UnityEngine;
using UnityEngine.InputSystem;

public class CrosshairController : MonoBehaviour
{
    [SerializeField] private GameObject _crosshairCenter;
    
    [SerializeField] private RectTransform _upRect;
    [SerializeField] private RectTransform _leftRect;
    [SerializeField] private RectTransform _rightRect;
    [SerializeField] private RectTransform _downRect;

    [Space(10)]
    [Header("Spread Setting")]
    [SerializeField] private float _defaultSpread;
    [SerializeField] private float _aimingSpread;
    [SerializeField] private float _maxSpread;
    [SerializeField] private float _spreadPerShot;
    [SerializeField] private float _spreadRecoverSpeed;

    [Space(10)]
    [Header("Recoil Setting")]
    [SerializeField] private float _defaultRecoilAmount;
    [SerializeField] private float _aimingRecoilAmount;
    [SerializeField] private float _recoilSpeed;
    [SerializeField] private float _recoilReturnSpeed;

    [Space(10)]
    [Header("Shake Setting")]
    [SerializeField] private float _shakeAmount;
    [SerializeField] private float _shakeDuration;
    [SerializeField] private float _shakeInterval;


    private GameManager _gameManager;
    private PlayerShooting _playerShooting;
    private RectTransform _rect;
    private InputActions _actions;

    private float _spreadAmountToApply;
    private float _recoilAmountToApply;

    private float _currentSpread;

    private float _shakeTimer = 0f;
    private float _shakeIntervalTimer = 0f;
    private float _shakeOffset = 0f;

    private Vector2 _recoilOffset = Vector2.zero;
    private Vector2 _targetRecoilOffset = Vector2.zero;
    private Vector2 _mousePos;

    public float CurrentSpread => _currentSpread;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();

        _spreadAmountToApply = _defaultSpread;
        _recoilAmountToApply = _defaultRecoilAmount;

        _currentSpread = _defaultSpread;
        _crosshairCenter.SetActive(false);
    }

    private void OnEnable()
    {
        SubscribeInputActions();

        if (_playerShooting != null)
        {
            _playerShooting.OnFire += ApplyRecoil;
            _playerShooting.OnFireFail += FailFire;
        }
    }

    private void Start()
    {
        _gameManager = GameManager.Instance;

        _actions = _gameManager.Actions;
        SubscribeInputActions();

        UIManager.Instance.ShowCursor(false);

        _playerShooting = _gameManager.PlayerObject.GetComponent<PlayerShooting>();

        _playerShooting.OnFire += ApplyRecoil;
        _playerShooting.OnFireFail += FailFire;
    }

    private void Update()
    {
        UpdateShakeOffest();

        _mousePos = Mouse.current.position.ReadValue();
        _rect.position = _mousePos + _recoilOffset;
        _rect.localRotation = Quaternion.Euler(0f, 0f, _shakeOffset);

        UpdateRecoil();
        UpdateSpread();
    }

    private void OnDisable()
    {
        _actions.Player.Aim.performed -= OnAimPerformed;
        _actions.Player.Aim.canceled -= OnAimCanceled;

        _playerShooting.OnFire -= ApplyRecoil;
        _playerShooting.OnFireFail -= FailFire;
    }

    private void SubscribeInputActions()
    {
        if (_actions != null)
        {
            _actions.Player.Aim.performed += OnAimPerformed;
            _actions.Player.Aim.canceled += OnAimCanceled;
        }
    }

    private void OnAimPerformed(InputAction.CallbackContext context)
    {
        _crosshairCenter.SetActive(true);
        _spreadAmountToApply = _aimingSpread;
        _recoilAmountToApply = _aimingRecoilAmount;
    }

    private void OnAimCanceled(InputAction.CallbackContext context)
    {
        _crosshairCenter.SetActive(false);
        _spreadAmountToApply = _defaultSpread;
        _recoilAmountToApply = _defaultRecoilAmount;
    }

    private void UpdateRecoil()
    {
        _recoilOffset = Vector2.Lerp(_recoilOffset, _targetRecoilOffset, Time.deltaTime * _recoilSpeed);

        _targetRecoilOffset = Vector2.Lerp(_targetRecoilOffset, Vector2.zero, Time.deltaTime * _recoilReturnSpeed);
    }

    private void UpdateSpread()
    {
        _currentSpread = Mathf.Lerp(_currentSpread, _spreadAmountToApply, Time.deltaTime * _spreadRecoverSpeed);

        _upRect.anchoredPosition = new Vector2(_upRect.anchoredPosition.x, _currentSpread);
        _leftRect.anchoredPosition = new Vector2(-_currentSpread, _leftRect.anchoredPosition.y);
        _rightRect.anchoredPosition = new Vector2(_currentSpread, _rightRect.anchoredPosition.y);
        _downRect.anchoredPosition = new Vector2(_downRect.anchoredPosition.x, -_currentSpread);
    }

    private void UpdateShakeOffest()
    {
        if (_shakeTimer <= 0f)
        {
            _shakeOffset = 0f;
            return;
        }

        _shakeTimer -= Time.deltaTime;
        _shakeIntervalTimer -= Time.deltaTime;

        if (_shakeIntervalTimer <= 0f)
        {
            // 발사 직후 강하고 서서히 약해지게
            float amount = _shakeAmount * (_shakeTimer / _shakeDuration);

            _shakeOffset = Random.Range(-amount, amount);

            _shakeIntervalTimer = _shakeInterval;
        }
    }


    private float ApplyRecoil()
    {
        Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);

        Vector2 recoilDir = (_mousePos - screenCenter).normalized;

        _targetRecoilOffset += recoilDir * _recoilAmountToApply;

        _currentSpread = Mathf.Min(_currentSpread + _spreadPerShot, _maxSpread);

        _shakeTimer += _shakeDuration;

        return _currentSpread;
    }

    private void FailFire()
    {
        _targetRecoilOffset = Vector2.zero;
        _currentSpread = _defaultSpread;
        _shakeTimer = 0f;
        _shakeIntervalTimer = 0f;
    }
}
