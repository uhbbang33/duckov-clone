using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StaminaPoint : MonoBehaviour
{
    private float _maxSP;
    private float _currentSP;
    private bool _isReducing;
    Hydration _hydration;

    [SerializeField] private float _runDelayTime;
    [SerializeField] private float _healFirstDelayTime;
    [SerializeField] private Slider _SPSlider;
    [SerializeField] private Image _SPSliderFillImage;

    private Player _player;
    private PlayerBaseData _playerBaseData;

    private Color _originSliderColor;
    private float _currentHealAmountPerTick;

    private WaitForSeconds _waitRunDelay;
    private WaitForSeconds _waitHealDelay;
    private WaitForSeconds _waitHealFirstDelay;

    private Coroutine _reduceRoutine = null;
    private Coroutine _healRoutine = null;

    private const float _changeBackgroundColorAmount = 30f;

    public float CurrentSP { get { return _currentSP; } }
    public bool IsReducing { set { _isReducing = value; } }

    public event Action OnSPZero;

    private void Awake()
    {
        _player = GetComponent<Player>();
        _player.OnPlayerDataInitialized += PlayerBaseDataSetup;

        _hydration = GetComponent<Hydration>();

        _originSliderColor = _SPSliderFillImage.color;
        _isReducing = false;
        _waitRunDelay = new WaitForSeconds(_runDelayTime);
        _waitHealFirstDelay = new WaitForSeconds(_healFirstDelayTime);
    }

    private void Start()
    {
        _hydration.OnEnterZeroHydration += HalveHealAmountPerTick;
        _hydration.OnExitZeroHydration += RestoreHealAmountPerTick;
    }

    private void OnDisable()
    {
        _hydration.OnEnterZeroHydration -= HalveHealAmountPerTick;
        _hydration.OnExitZeroHydration -= RestoreHealAmountPerTick;
    }

    private void PlayerBaseDataSetup()
    {
        _playerBaseData = _player.BaseData;

        _maxSP = _playerBaseData.MaxSP;
        _currentSP = _maxSP;

        _currentHealAmountPerTick = _playerBaseData.SPRegenAmount;

        _waitHealDelay = new WaitForSeconds(_playerBaseData.SPRegenInterval);

        ChangeSlider();
    }

    public void ReduceSPImmediately(float amount)
    {
        _currentSP -= amount;

        if (_currentSP <= 0)
        {
            _currentSP = 0;

            OnSPZero?.Invoke();
        }

        if (_healRoutine != null)
            StopCoroutine(_healRoutine);

        if (_reduceRoutine != null)
            _healRoutine = StartCoroutine(HealRoutine());

        _currentSP = MathF.Round(_currentSP, 2);

        ChangeSlider();

    }

    public void ReduceSPPerSecond(float amount)
    {
        _isReducing = true;

        if (_reduceRoutine != null)
            StopCoroutine(_reduceRoutine);

        _reduceRoutine = StartCoroutine(ReducePerSecondRoutine(amount));
    }

    private void HealStamina(float healAmount)
    {
        _currentSP += healAmount;

        if (_currentSP > _maxSP)
            _currentSP = _maxSP;

        if (_currentSP == _maxSP)
        {
            _SPSlider.gameObject.SetActive(false);
            return;
        }

        _currentSP = MathF.Round(_currentSP, 2);

        ChangeSlider();
    }

    private void ChangeSlider()
    {
        ChangeSPSliderValue();

        if (_currentSP <= _changeBackgroundColorAmount)
        {
            Color newColor = new Color32(255, 153, 153, 255);
            _SPSliderFillImage.color = newColor;
        }
        else
            _SPSliderFillImage.color = _originSliderColor;
    }

    private void ChangeSPSliderValue()
    {
        if (!_SPSlider.gameObject.activeSelf)
            _SPSlider.gameObject.SetActive(true);

        _SPSlider.value = _currentSP / _maxSP;
    }

    private void HalveHealAmountPerTick()
    {
        _currentHealAmountPerTick = _playerBaseData.SPRegenAmount / 2;
    }

    private void RestoreHealAmountPerTick()
    {
        _currentHealAmountPerTick = _playerBaseData.SPRegenAmount;
    }



    #region Coroutine

    private IEnumerator ReducePerSecondRoutine(float amount)
    {
        while (_isReducing)
        {
            ReduceSPImmediately(amount);
            yield return _waitRunDelay;
        }

        yield return null;
    }

    private IEnumerator HealRoutine()
    {
        yield return _waitHealFirstDelay;

        while (_currentSP < _maxSP)
        {
            HealStamina(_currentHealAmountPerTick);
            yield return _waitHealDelay;
        }
        
        yield return null;
    }

    #endregion
}
