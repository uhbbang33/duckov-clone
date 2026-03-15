using UnityEngine;
using UnityEngine.UI;

public class HealthPoint : MonoBehaviour
{
    [SerializeField] protected float _maxHP;
    [SerializeField] private Slider _HPBarSlider;

    protected float _currentHP;
    public float CurrentHP
    {
        get { return _currentHP; }
    }

    public float MaxHP
    {
        get { return _maxHP; }
        set { _maxHP = value; }
    }

    public bool Heal(float amount)
    {
        if (_currentHP == _maxHP)
            return false;

        _currentHP += amount;

        if (_currentHP > _maxHP)
            _currentHP = _maxHP;

        ChangeHPSliderValue();
        return true;
    }

    public void TakeDamage(float damageAmount)
    {
        _currentHP -= damageAmount;

        if (_currentHP <= 0)
        {
            _currentHP = 0;
            
            Death();
        }

        ChangeHPSliderValue();
    }

    protected virtual void Death()
    {

    }

    protected virtual void ChangeHPSliderValue()
    {
        _HPBarSlider.value = _currentHP / _maxHP;
    }
}
