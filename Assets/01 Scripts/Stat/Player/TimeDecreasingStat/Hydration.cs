using System;

public class Hydration : TimeDecreasingStat
{
    public event Action OnEnterZeroHydration;
    public event Action OnExitZeroHydration;

    protected override void PlayerBaseDataSetup()
    {
        base.PlayerBaseDataSetup();

        _max = _playerBaseData.MaxHydration;
        _current = _max;

        _originReduceAmountPerTick = _playerBaseData.HydrationLossPerSec;
        _currentReduceAmountPerTick = _originReduceAmountPerTick;
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
