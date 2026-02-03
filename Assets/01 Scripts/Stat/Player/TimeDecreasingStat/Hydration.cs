
using System;

public class Hydration : TimeDecreasingStat
{
    public event Action OnZeroHydration;

    protected override void RefreshUI()
    {
        UIManager.Instance.ChangeMainUIHydrationSlider(_current, _max);
    }

    protected override void OnEnterZeroStat()
    {
        OnZeroHydration?.Invoke();
    }

    protected override void OnExitZeroStat()
    {

    }

}
