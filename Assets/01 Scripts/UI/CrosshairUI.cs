using UnityEngine;
using UnityEngine.InputSystem;

public class CrosshairUI : MonoBehaviour
{
    [SerializeField] private GameObject _crosshairCenter;
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
        ChangePosition(_originPosition);
    }

    private void OnEnable()
    {
        SubscribeInputActions();
    }

    private void Start()
    {
        _actions = GameManager.Instance.PlayerObject.GetComponent<Player>().Actions;
        SubscribeInputActions();

        UIManager.Instance.ShowCursor(false);
    }


    private void OnDisable()
    {
        _actions.Player.Aim.performed -= OnAimPerformed;
        _actions.Player.Aim.canceled -= OnAimCanceled;
    }

    private void Update()
    {
        _rect.position = Mouse.current.position.ReadValue();
    }

    private void SubscribeInputActions()
    {
        if (_actions != null)
        {
            _actions.Player.Aim.performed += OnAimPerformed;
            _actions.Player.Aim.canceled += OnAimCanceled;
        }
    }

    private void OnAimPerformed(InputAction.CallbackContext context)
    {
        ChangePosition(_aimingPosition);
        //_crosshairCenter.SetActive(true);
    }

    private void OnAimCanceled(InputAction.CallbackContext context)
    {
        ChangePosition(_originPosition);
        //_crosshairCenter.SetActive(false);
    }

    private void ChangePosition(float posOffest)
    {
        _upRect.anchoredPosition = new Vector2(_upRect.anchoredPosition.x, posOffest);
        _leftRect.anchoredPosition = new Vector2(-posOffest, _leftRect.anchoredPosition.y);
        _rightRect.anchoredPosition = new Vector2(posOffest, _rightRect.anchoredPosition.y);
        _downRect.anchoredPosition = new Vector2(_downRect.anchoredPosition.x, -posOffest);
    }

}
