using UnityEngine;

public class HealthPoint : MonoBehaviour
{
    // TODO: Json Data
    [SerializeField] private float _maxHP;
    private float _currentHP;
    public float CurrentHP
    {
        get { return _currentHP; }
    }

    private void Start()
    {
        _currentHP = _maxHP;
    }

    public void TakeDamage(float damageAmount)
    {
        _currentHP -= damageAmount;

        if(_currentHP <= 0)
        {
            _currentHP = 0;

            // TODO die
        }
    }
}
