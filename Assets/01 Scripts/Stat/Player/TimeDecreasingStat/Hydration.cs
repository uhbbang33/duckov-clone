using System;

public class Hydration : TimeDecreasingStat
{
    public event Action OnEnterZeroHydration;
    public event Action OnExitZeroHydration;

    protected override void PlayerSetup()
    {
        base.PlayerSetup();

        _max = _playerBaseData.MaxHydration;
        Heal(_max);

        _originReduceAmountPerTick = _playerBaseData.HydrationLossPerSec;
        _currentReduceAmountPerTick = _originReduceAmountPerTick;

        OnExitZeroStat();
    }

    protected override void RefreshUI()
    {
        _uiManager.ChangeMainUIHydrationSlider(_current, _max);
    }

    protected override void OnEnterZeroStat()
    {
        base.OnEnterZeroStat();

        _uiManager.ChangeHydrationSliderBackgroundColor(true);

        OnEnterZeroHydration?.Invoke();
    }

    protected override void OnExitZeroStat()
    {
        base.OnExitZeroStat();
        
        _uiManager.ChangeHydrationSliderBackgroundColor(false);

        OnExitZeroHydration?.Invoke();
    }

}
