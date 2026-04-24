using UnityEngine;

public class StencilMask : MonoBehaviour
{
    [SerializeField] private float _maskRadius;
    [SerializeField] protected float _distanceFromCamera;

    protected virtual void Start()
    {
        //transform.localScale = Vector3.one * _maskRadius;
        transform.localScale = new Vector3(_maskRadius, transform.localScale.y, _maskRadius);
    }
}
