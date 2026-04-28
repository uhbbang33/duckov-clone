using UnityEngine;

public class SetWorldSpaceCanvas : MonoBehaviour
{
    [SerializeField] private GameObject _canvas;
    [SerializeField] private float _scaleOffset = 0.005f;
    [SerializeField] private Vector3 _positionOffest;
    
    void Start()
    {
        SetScale();
    }

    private void SetScale()
    {
        Vector3 scale = transform.localScale;

        _canvas.transform.localScale = new Vector3(1f / scale.x, 1f / scale.y, 1f / scale.z) * _scaleOffset;

        _canvas.transform.position = transform.position + _positionOffest;
    }
}
