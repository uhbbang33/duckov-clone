using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class StaminaPoint : MonoBehaviour
{
    private float _currentSP;
    private bool _isReducing;

    // TODO: Json Data
    [SerializeField] private float _maxSP;
    [SerializeField] private float _healAmountPerTick;
    [SerializeField] private float _runDelayTime;
    [SerializeField] private float _healDelayTime;
    [SerializeField] private float _healFirstDelayTime;
    [SerializeField] private Slider _SPSlider;

    private WaitForSeconds _waitRunDelay;
    private WaitForSeconds _waitHealDelay;
    private WaitForSeconds _waitHealFirstDelay;

    private Coroutine _reduceRoutine = null;
    private Coroutine _healRoutine = null;

    public float CurrentSP { get { return _currentSP; } }
    public bool IsReducing { set { _isReducing = value; } }

    public event Action OnSPZero;

    private void Awake()
    {
        _currentSP = _maxSP;
        _isReducing = false;
        _waitRunDelay = new WaitForSeconds(_runDelayTime);
        _waitHealDelay = new WaitForSeconds(_healDelayTime);
        _waitHealFirstDelay = new WaitForSeconds(_healFirstDelayTime);
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

        ChangeSPSliderValue();
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

        ChangeSPSliderValue();
    }

    private void ChangeSPSliderValue()
    {
        if (!_SPSlider.gameObject.activeSelf)
            _SPSlider.gameObject.SetActive(true);

        _SPSlider.value = _currentSP / _maxSP;
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
            HealStamina(_healAmountPerTick);
            yield return _waitHealDelay;
        }
        
        yield return null;
    }

    #endregion
}
