using TMPro;
using UnityEngine;

public class TimeUI : MonoBehaviour
{
    [SerializeField] private DayNightCycle _dayNightCycle;
    [SerializeField] private TextMeshProUGUI _timeText;

    private void Update()
    {
        float currentTime = _dayNightCycle.CurrentTime;

        int hours = Mathf.FloorToInt(currentTime);
        int minutes = Mathf.FloorToInt(currentTime % 1f * 60f);

        _timeText.text = $"{hours:D2}:{minutes:D2}";
    }
}
