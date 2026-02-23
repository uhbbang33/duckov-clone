using UnityEngine;
using UnityEngine.InputSystem;

public class CrosshairUI : MonoBehaviour
{
    [SerializeField] private RectTransform _upRect;
    [SerializeField] private RectTransform _leftRect;
    [SerializeField] private RectTransform _rightRect;
    [SerializeField] private RectTransform _downRect;
    [SerializeField] private float _originPosition;
    [SerializeField] private float _aimingPosition;

    private RectTransform _rect;
    private InputActions _actions;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
    }

    private void Start()
    {
        _actions = GameManager.Instance.PlayerObject.GetComponent<Player>().Actions;

        _actions.Player.Aim.performed += OnAim;

        UIManager.Instance.ShowCursor(false);
    }


    private void OnDisable()
    {
        _actions.Player.Aim.performed -= OnAim;
    }

    private void Update()
    {
        _rect.position = Mouse.current.position.ReadValue();
    }

    private void OnAim(InputAction.CallbackContext context)
    {
        ChangePosition(_aimingPosition);
    }

    private void ChangePosition(float posOffest)
    {
        
    }

}
