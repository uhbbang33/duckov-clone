using System;

public class Hunger : TimeDecreasingStat
{
    public event Action OnEnterZeroHunger;
    public event Action OnExitZeroHunger;


    protected override void PlayerSetup()
    {
        base.PlayerSetup();

        _max = _playerBaseData.MaxHunger;
        Heal(_max);

        _originReduceAmountPerTick = _playerBaseData.HungerLossPerSec;
        _currentReduceAmountPerTick = _originReduceAmountPerTick;
    }

    protected override void RefreshUI()
    {
        _uiManager.ChangeMainUIHungerSlider(_current, _max);
    }

    protected override void OnEnterZeroStat()
    {
        base.OnEnterZeroStat();

        _uiManager.ChangeHungerSliderBackgroundColor(true);

        OnEnterZeroHunger?.Invoke();
    }

    protected override void OnExitZeroStat()
    {
        base.OnExitZeroStat();

        _uiManager.ChangeHungerSliderBackgroundColor(false);

        OnExitZeroHunger?.Invoke();
    }
}
