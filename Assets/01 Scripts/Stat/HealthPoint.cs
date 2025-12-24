using UnityEngine;
using UnityEngine.UI;

public class HealthPoint : MonoBehaviour
{
    // TODO: Json Data
    [SerializeField] private float _maxHP;
    [SerializeField] private Slider _HPBarSlider;

    private float _currentHP;
    public float CurrentHP
    {
        get { return _currentHP; }
    }

    private void Start()
    {
        _currentHP = _maxHP;
        ChangeHPSliderValue();
    }

    public void TakeDamage(float damageAmount)
    {
        _currentHP -= damageAmount;

        if (_currentHP <= 0)
        {
            _currentHP = 0;

            // TODO die
        }

        ChangeHPSliderValue();
    }

    private void ChangeHPSliderValue()
    {
        _HPBarSlider.value = _currentHP / _maxHP;
    }
}
