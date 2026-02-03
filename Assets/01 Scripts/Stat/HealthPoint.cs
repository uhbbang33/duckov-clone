using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthPoint : MonoBehaviour
{
    [SerializeField] protected float _maxHP;
    [SerializeField] private Slider _HPBarSlider;


    [Space(10)]
    [Header("FOR TEST")]
    [SerializeField] private float _tempCurrentHP;

    protected float _currentHP;
    public float CurrentHP
    {
        get { return _currentHP; }
    }

    protected virtual void Awake()
    {
        //_currentHP = _maxHP;
        _currentHP = _tempCurrentHP;
    }

    protected virtual void Start()
    {
        ChangeHPSliderValue();
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

            // TODO die
        }

        ChangeHPSliderValue();
    }

    protected virtual void ChangeHPSliderValue()
    {
        _HPBarSlider.value = _currentHP / _maxHP;
    }
}
