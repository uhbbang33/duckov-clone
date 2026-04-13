using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class StencilMouseMask : StencilMask
{
    private InputActions _inputActions;
    private Vector3 _mousePosition;

    private Camera _mainCamera;

    protected override void Start()
    {
        base.Start();

        _inputActions = GameManager.Instance.Actions;
        _inputActions.Player.Look.performed += OnLook;

        _mainCamera = Camera.main;
    }

    void OnEnable()
    {
        CinemachineCore.CameraUpdatedEvent.AddListener(OnCameraUpdated);
    }


    private void OnDisable()
    {
        _inputActions.Player.Look.performed -= OnLook;

        CinemachineCore.CameraUpdatedEvent.RemoveListener(OnCameraUpdated);
    }

    private void OnCameraUpdated(CinemachineBrain brain)
    {
        Vector3 mouseScreen = new Vector3(_mousePosition.x, _mousePosition.y, _distanceFromCamera);
        transform.position = _mainCamera.ScreenToWorldPoint(mouseScreen);
    }

    private void OnLook(InputAction.CallbackContext context)
    {
        _mousePosition = context.ReadValue<Vector2>();
    }
}
