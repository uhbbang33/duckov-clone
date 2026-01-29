using System.Collections;
using UnityEngine;

public class Hydration : MonoBehaviour
{
    [SerializeField] private float _maxHydration;
    [SerializeField] private float _healAmountPerTick;
    [SerializeField] private float _reduceAmountPerTick;
    [SerializeField] private float _reduceDelay;

    private float _currentHydration;
    private WaitForSeconds _waitForReduceDelay;

    private void Awake()
    {
        _waitForReduceDelay = new WaitForSeconds(_reduceDelay);
        _currentHydration = _maxHydration;
    }

    private void Start()
    {
        StartCoroutine(ReducePerTickRoutine());
    }

    public void HealHydration(int amount)
    {
        _currentHydration += amount;
        if (_currentHydration > _maxHydration)
            _currentHydration = _maxHydration;

        RefreshUI();
    }

    private void RefreshUI()
    {
        UIManager.Instance.ChangeMainUIHydrationSlider(_currentHydration, _maxHydration);
    }

    private IEnumerator ReducePerTickRoutine()
    {
        while (_currentHydration > 0)
        {
            _currentHydration -= _reduceAmountPerTick;

            RefreshUI();

            yield return _waitForReduceDelay;
        }

        if(_currentHydration <= 0)
        {
            _currentHydration = 0;
            
            //TODO 수분이 고갈되었을 때

        }


        yield return null;
    }

}
