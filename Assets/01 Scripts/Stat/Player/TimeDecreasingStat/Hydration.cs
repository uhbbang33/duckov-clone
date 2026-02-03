
using System;

public class Hydration : TimeDecreasingStat
{
    public event Action OnEnterZeroHydration;
    public event Action OnExitZeroHydration;


    protected override void RefreshUI()
    {
        UIManager.Instance.ChangeMainUIHydrationSlider(_current, _max);
    }

    protected override void OnEnterZeroStat()
    {
        base.OnEnterZeroStat();

        OnEnterZeroHydration?.Invoke();
    }

    protected override void OnExitZeroStat()
    {
        base.OnExitZeroStat();

        OnExitZeroHydration?.Invoke();
    }

}
