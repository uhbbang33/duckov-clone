
using System;

public class Hunger : TimeDecreasingStat
{
    public event Action OnEnterZeroHunger;
    public event Action OnExitZeroHunger;

    protected override void RefreshUI()
    {
        UIManager.Instance.ChangeMainUIHungerSlider(_current, _max);
    }

    protected override void OnEnterZeroStat()
    {
        base.OnEnterZeroStat();

        OnEnterZeroHunger?.Invoke();
    }

    protected override void OnExitZeroStat()
    {
        base.OnExitZeroStat();

        OnExitZeroHunger?.Invoke();
    }
}
