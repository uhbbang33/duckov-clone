using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class DroppedItem : MonoBehaviour
{
    private Item _item;
    private int _quantity;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LayerMask _interactableLayer;

    public Item CurrentItem { get { return _item; } }
    public int Quantity { get { return _quantity; } }

    private Vector3 _positionOffset = new Vector3(0f, 0.6f, 0f);
    private const int _sectorAngle = 30;
    private const float _dropMinDistance = 0.5f;
    private const float _dropMaxDistance = 1f;

    public void InitializeDroppedItem(Item item, int quantity)
    {
        _item = item;
        _quantity = quantity;

        GetComponent<SpriteRenderer>().sprite = ItemSpriteDictionary.Instance.GetItemSprite(item.ID);

        SetPosition();
    }

    private void SetPosition()
    {
        Transform playerTransform = GameManager.Instance.PlayerObject.transform;

        const int tryNum = 5;

        for (int i = 0; i < tryNum; ++i)
        {
            float angle = Random.Range(-_sectorAngle, _sectorAngle);
            Vector3 direction = Quaternion.Euler(0f, angle, 0f) * playerTransform.forward;

            float distance = Random.Range(_dropMinDistance, _dropMaxDistance);
            Vector3 pos = playerTransform.position + direction * distance;

            if (!TryGetGroundPosition(pos, out Vector3 groundPos)
                || IsOverlapWithOthers(groundPos))
                continue;

            gameObject.transform.position = groundPos;
            break;
        }

    }

    private bool TryGetGroundPosition(Vector3 origin, out Vector3 groundPos)
    {
        if(Physics.Raycast(origin + Vector3.up, Vector3.down, out RaycastHit hit, 3f, _groundLayer))
        {
            groundPos = hit.point + _positionOffset;
            return true;
        }
        groundPos = Vector3.zero;
        return false;
    }

    private bool IsOverlapWithOthers(Vector3 pos)
    {
        return Physics.OverlapSphere(pos, 0.5f, _interactableLayer).Length != 0;
    }
}
