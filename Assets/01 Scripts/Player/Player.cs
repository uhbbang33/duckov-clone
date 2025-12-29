using UnityEngine;

public class Player : MonoBehaviour
{
    private InputActions _inputActions;
    public InputActions Actions { get { return _inputActions; } }

    private void Awake()
    {
        _inputActions = new InputActions();
        _inputActions.Player.Enable();
    }

    private void OnDestroy()
    {
        _inputActions.Player.Disable();
    }
}
