using System.Collections;
using UnityEngine;

public class TimeDecreasingStat : MonoBehaviour
{
    [SerializeField] protected float _max;
    [SerializeField] private float _reduceAmountPerTick;
    [SerializeField] private float _reduceDelay;

    protected float _current;
    private WaitForSeconds _waitForReduceDelay;

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

    public void Heal(int amount)
    {
        _current += amount;
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

            //TODO 에너지가 고갈되었을 때

        }


        yield return null;
    }
}
