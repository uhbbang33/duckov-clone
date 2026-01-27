using UnityEngine;

public class BoxSlotLoad : MonoBehaviour
{
    [SerializeField] private GameObject _unloadedImage;
    [SerializeField] private GameObject _loadingIcon;

    [SerializeField] private float _iconRotateSpeed = 5f;
    [SerializeField] private float _iconRotateRadius = 10f;

    private Vector3 _centerPosition;
    float _angle = 0f;

    private void OnEnable()
    {
        _centerPosition = gameObject.transform.position;

        _angle = 0f;
        SetIconPosition(_angle);
    }

    private void Update()
    {
        if (_loadingIcon.activeSelf)
            IconAnimation();
    }

    private void IconAnimation()
    {
        _angle -= _iconRotateSpeed * Time.deltaTime;
        SetIconPosition(_angle);
    }

    private void SetIconPosition(float angle)
    {
        float x = _centerPosition.x + Mathf.Cos(angle) * _iconRotateRadius;
        float y = _centerPosition.y + Mathf.Sin(angle) * _iconRotateRadius;

        _loadingIcon.transform.position = new Vector3(x, y, _loadingIcon.transform.position.z);
    }

    public void SetEmptySlot()
    {
        _unloadedImage.SetActive(false);
        _loadingIcon.SetActive(false);
    }

    public void SetItemSlotBeforeLoad()
    {
        _unloadedImage.SetActive(true);
    }

    public void StartLoad()
    {
        _loadingIcon.SetActive(true);
    }

    public void LoadComplete()
    {
        _loadingIcon.SetActive(false);
        _unloadedImage.SetActive(false);
    }
}
