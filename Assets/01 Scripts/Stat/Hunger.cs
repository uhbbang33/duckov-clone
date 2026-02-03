
public class Hunger : TimeDecreasingStat
{
    protected override void RefreshUI()
    {
        UIManager.Instance.ChangeMainUIHungerSlider(_current, _max);
    }
}
