using UnityEngine;
using UnityEngine.InputSystem;

public class BuildingMouseDetector : MonoBehaviour
{
    [SerializeField] private LayerMask _buildingMask;
    [SerializeField] private float _rayMaxDistance;

    private RendererHider _prevHoveredBuilding;

    void Update()
    {
        DetectBuildingUnderMouse();
    }

    private void DetectBuildingUnderMouse()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        RendererHider current = null;

        if(Physics.Raycast(ray, out RaycastHit hit, _rayMaxDistance, _buildingMask))
        {
            current = hit.collider.GetComponent<RendererHider>();
        }

        if (current == _prevHoveredBuilding)
            return;

        if (_prevHoveredBuilding != null)
            _prevHoveredBuilding.MouseHover(false);

        if (current != null)
            current.MouseHover(true);

        _prevHoveredBuilding = current;
    }
}
