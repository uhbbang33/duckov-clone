using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class DroppedItem : MonoBehaviour
{
    private Item _item;
    private int _quantity;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LayerMask _interactableLayer;
    [SerializeField] private Vector3 _dropPositionOffset = new Vector3(0f, 0.6f, 0f);

    [SerializeField] private int _sectorAngle = 90;
    [SerializeField] private float _dropMinDistance = 0.5f;
    [SerializeField] private float _dropMaxDistance = 1.5f;
    [SerializeField] private float _overlapDistance = 0.1f;

    public Item CurrentItem { get { return _item; } }
    public int Quantity { get { return _quantity; } }


    public event Action FinshInitialize;

    public bool InitializeDroppedItem(Item item, int quantity)
    {
        _item = item;
        _quantity = quantity;

        GetComponent<SpriteRenderer>().sprite = ItemSpriteDictionary.Instance.GetItemSprite(item.ID);

        if (!TrySetPosition())
            return false;

        FinshInitialize?.Invoke();

        transform.forward = Camera.main.transform.forward;

        return true;
    }

    private bool TrySetPosition()
    {
        Transform playerTransform = GameManager.Instance.PlayerObject.transform;

        const int tryNum = 20;

        for (int i = 0; i < tryNum; ++i)
        {
            float angle;
            if (i < tryNum / 2)
                angle = UnityEngine.Random.Range(-_sectorAngle, _sectorAngle);
            else
                angle = UnityEngine.Random.Range(_sectorAngle, 360 - _sectorAngle);

            Vector3 direction = Quaternion.Euler(0f, angle, 0f) * playerTransform.forward;

            float distance = UnityEngine.Random.Range(_dropMinDistance, _dropMaxDistance);
            Vector3 pos = playerTransform.position + direction * distance;

            if (!TryGetGroundPosition(pos, out Vector3 groundPos)
                || IsOverlapWithOthers(groundPos))
                continue;

            gameObject.transform.position = groundPos;
            //Debug.Log("½ÃµµÈ½¼ö: " + i);

            return true;
        }

        return false;
    }

    private bool TryGetGroundPosition(Vector3 origin, out Vector3 groundPos)
    {
        if(Physics.Raycast(origin + Vector3.up, Vector3.down, out RaycastHit hit, 3f, _groundLayer))
        {
            groundPos = hit.point + _dropPositionOffset;
            return true;
        }
        groundPos = Vector3.zero;
        return false;
    }

    private bool IsOverlapWithOthers(Vector3 pos)
    {
        return Physics.OverlapSphere(pos, _overlapDistance, _interactableLayer).Length != 0;
    }

    public void OnInteract()
    {
        GameManager.Instance.Inventory.TryAddItem(_item, _quantity);

        Destroy(gameObject);
    }
}
