using UnityEngine;

public class Player : MonoBehaviour
{
    private InputActions _inputActions;
    public InputActions Actions { get { return _inputActions; } }

    private HealthPoint _hp;
    private StaminaPoint _sp;
    private Hunger _hunger;
    private Hydration _hydration;

    private void Awake()
    {
        _inputActions = new InputActions();
        _inputActions.Player.Enable();

        _hp = GetComponent<HealthPoint>();
        _sp = GetComponent<StaminaPoint>();
        _hunger = GetComponent<Hunger>();
        _hydration = GetComponent<Hydration>();
    }

    private void OnDestroy()
    {
        _inputActions.Player.Disable();
    }

    public bool UseItem(UsableItem item)
    {
        if (!_hp.Heal(item.HealHP))
        {
            return false;
        }

        _hunger.Heal(item.Hunger);
        _hydration.Heal(item.Hydration);

        return true;
    }
}
