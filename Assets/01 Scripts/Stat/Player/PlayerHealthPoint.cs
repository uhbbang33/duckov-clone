using System.Collections;
using UnityEngine;

public class PlayerHealthPoint : HealthPoint
{
    [SerializeField] private float _reduceDelay;
    [SerializeField] private float _reduceAmountPerTick;

    private WaitForSeconds _waitForReduceDelay;

    protected override void Awake()
    {
        base.Awake();

        _waitForReduceDelay = new WaitForSeconds(_reduceDelay);
    }

    protected override void Start()
    {
        base.Start();

        GetComponent<Hunger>().OnEnterZeroHunger += StartReduceHP;
        GetComponent<Hunger>().OnExitZeroHunger += StopReduceHP;

    }

    private void StartReduceHP()
    {
        StartCoroutine(ReduceHPRoutine());
    }

    private void StopReduceHP()
    {
        StopCoroutine(ReduceHPRoutine());
    }

    protected override void ChangeHPSliderValue()
    {
        base.ChangeHPSliderValue();

        UIManager.Instance.ChangeMainUIHPBar(_currentHP, _maxHP);
    }

    private IEnumerator ReduceHPRoutine()
    {
        while(_currentHP > 0)
        {
            TakeDamage(_reduceAmountPerTick);

            yield return _waitForReduceDelay;
        }

        yield return null;
    }


}
