using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Range(0f, 24f)]
    [SerializeField] private float _currentTime;
    [SerializeField] private float _dayDurationSeconds; // 실제 초 기준 하루 길이
    [SerializeField] private Light _sunLight;
    [SerializeField] private Gradient _sunColor; // 시간대별 색상
    [SerializeField] private float _lightYRotate = -30f;

    private bool _isFieldScene;

    private const float _hoursPerDay = 24f;

    public float CurrentTime => _currentTime;

    private void Start()
    {
        if (GameManager.Instance.CurrentSceneName == SceneName.FieldScene)
            _isFieldScene = true;
        else
            _isFieldScene = false;
    }

    private void Update()
    {
        _currentTime += (_hoursPerDay / _dayDurationSeconds) * Time.deltaTime;
        if (_currentTime >= _hoursPerDay)
            _currentTime = 0f;

        float t = _currentTime / _hoursPerDay;

        if (!_isFieldScene)
            return;

        // 빛(태양) 회전
        float sunAngle = t * 360f - 90f;
        _sunLight.transform.rotation = Quaternion.Euler(sunAngle, _lightYRotate, 0f);

        _sunLight.color = _sunColor.Evaluate(t);
    }
}
