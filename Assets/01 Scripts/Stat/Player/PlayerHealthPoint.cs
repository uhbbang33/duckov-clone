using System.Collections;
using UnityEngine;

public class PlayerHealthPoint : HealthPoint
{
    [SerializeField] private float _reduceDelay;
    [SerializeField] private float _reduceAmountPerTick;

    private bool _isReducingByHungerZero;
    private Hunger _hunger;
    private WaitForSeconds _waitForReduceDelay;
    private Player _player;

    private PlayerBaseData _playerBaseData;


    private void Awake()
    {
        _waitForReduceDelay = new WaitForSeconds(_reduceDelay);
        _player = GetComponent<Player>();
        _player.OnPlayerDataInitialized += PlayerBaseDataSetup;
    }

    private void Start()
    {
        _hunger = GetComponent<Hunger>();
        _hunger.OnEnterZeroHunger += StartReduceHP;
        _hunger.OnExitZeroHunger += StopReduceHP;
    }

    private void OnDisable()
    {
        _hunger.OnEnterZeroHunger -= StartReduceHP;
        _hunger.OnExitZeroHunger -= StopReduceHP;
    }

    private void PlayerBaseDataSetup()
    {
        _playerBaseData = _player.BaseData;

        _maxHP = _playerBaseData.MaxHP;
        _currentHP = _maxHP;
        
        ChangeHPSliderValue();
    }

    private void StartReduceHP()
    {
        _isReducingByHungerZero = true;
        StartCoroutine(ReduceHPRoutine());
    }

    private void StopReduceHP()
    {
        _isReducingByHungerZero = false;
    }

    protected override void Death()
    {
        base.Death();

        _player.ChangePlayerState(PlayerState.Die);
    }

    protected override void ChangeHPSliderValue()
    {
        base.ChangeHPSliderValue();

        UIManager.Instance.ChangeMainUIHPBar(_currentHP, _maxHP);
    }

    private IEnumerator ReduceHPRoutine()
    {
        while (_isReducingByHungerZero)
        {
            if (_currentHP > 0)
                TakeDamage(_reduceAmountPerTick);

            yield return _waitForReduceDelay;
        }
    }

    public void LoadCurrentHPData(float currentHP)
    {
        _currentHP = currentHP;

        ChangeHPSliderValue();
    }
}
