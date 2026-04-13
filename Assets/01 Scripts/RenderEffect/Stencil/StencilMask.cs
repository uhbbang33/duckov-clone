using UnityEngine;

public class StencilMask : MonoBehaviour
{
    [SerializeField] private float _maskRadius;
    [SerializeField] protected float _distanceFromCamera;

    protected virtual void Start()
    {
        transform.localScale = Vector3.one * _maskRadius;
    }
}
