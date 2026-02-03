
public class Hydration : TimeDecreasingStat
{
    protected override void RefreshUI()
    {
        UIManager.Instance.ChangeMainUIHydrationSlider(_current, _max);
    }
}
