using UnityEngine;

public class Player : MonoBehaviour
{
    private InputActions _inputActions;
    public InputActions Actions { get { return _inputActions; } }

    private HealthPoint _hp;
    private StaminaPoint _sp;

    private void Awake()
    {
        _inputActions = new InputActions();
        _inputActions.Player.Enable();

        _hp = GetComponent<HealthPoint>();
        _sp = GetComponent<StaminaPoint>();
    }

    private void OnDestroy()
    {
        _inputActions.Player.Disable();
    }

    public void UseItem(UsableItem item)
    {
        _hp.Heal(item.HealHP);

        //item.Hunger;

        //item.Hydration;

    }
}
