using System.Collections;
using UnityEngine;

public class TimeDecreasingStat : MonoBehaviour
{
    [SerializeField] protected float _max;
    [SerializeField] private float _reduceAmountPerTick;
    [SerializeField] private float _reduceDelay;

    protected float _current;
    private WaitForSeconds _waitForReduceDelay;
    protected bool _isZeroStat;

    private void Awake()
    {
        _waitForReduceDelay = new WaitForSeconds(_reduceDelay);
        _current = _max;
    }

    protected virtual void Start()
    {
        StartCoroutine(ReducePerTickRoutine());

        GetComponent<PlayerMove>().OnRun += EnableDouble;
        GetComponent<PlayerMove>().OnRunCancel += DisableDouble;
    }

    protected virtual void RefreshUI() { }

    protected virtual void OnEnterZeroStat() { }

    protected virtual void OnExitZeroStat() { }

    public void Heal(int amount)
    {
        _current += amount;
        OnExitZeroStat();

        if (_current > _max)
            _current = _max;

        RefreshUI();
    }

    private void EnableDouble()
    {
        _reduceAmountPerTick *= 2;
    }

    private void DisableDouble()
    {
        _reduceAmountPerTick /= 2;
    }

    private IEnumerator ReducePerTickRoutine()
    {
        while (_current > 0)
        {
            _current -= _reduceAmountPerTick;

            RefreshUI();

            yield return _waitForReduceDelay;
        }

        if (_current <= 0)
        {
            _current = 0;
            OnEnterZeroStat();
        }

        yield return null;
    }

}
