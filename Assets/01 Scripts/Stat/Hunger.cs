using System.Collections;
using UnityEngine;

public class Hunger : MonoBehaviour
{
    [SerializeField] private float _max;
    [SerializeField] private float _healAmountPerTick;
    [SerializeField] private float _reduceAmountPerTick;
    [SerializeField] private float _reduceDelay;

    private float _current;
    private WaitForSeconds _waitForReduceDelay;

    private void Awake()
    {
        _waitForReduceDelay = new WaitForSeconds(_reduceDelay);
        _current = _max;
    }

    private void Start()
    {
        StartCoroutine(ReducePerTickRoutine());
    }

    public void Heal(int amount)
    {
        _current += amount;
        if (_current > _max)
            _current = _max;

        RefreshUI();
    }

    private void RefreshUI()
    {
        UIManager.Instance.ChangeMainUIHungerSlider(_current, _max);
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
