using UnityEngine;
using UnityEngine.UI;

public class BoxSlotLoad : MonoBehaviour
{
    [SerializeField] private Image _unloadedImage;
    [SerializeField] private GameObject _loadingIcon;

    [SerializeField] private float _iconRotateSpeed = 5f;
    [SerializeField] private float _iconRotateRadius = 10f;

    private Vector3 _centerPosition;
    private float _angle = 0f;

    private float _loadingTime;

    public float LoadingTime { get { return _loadingTime; } }

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
        _unloadedImage.gameObject.SetActive(false);
        _loadingIcon.SetActive(false);
    }

    public void SetItemSlotBeforeLoad(string rarity)
    {
        _loadingTime = SetLoadingTime(rarity);

        _unloadedImage.gameObject.SetActive(true);
        UIManager.Instance.ChangeImageAlpha(_unloadedImage, true);
        _unloadedImage.gameObject.transform.SetAsLastSibling();
    }

    public void StartLoad()
    {
        _loadingIcon.SetActive(true);
    }

    public void LoadComplete()
    {
        UIManager.Instance.ChangeImageAlpha(_unloadedImage, false);
        _loadingIcon.SetActive(false);
    }

    public void AllBoxSlotsLoaded()
    {
        _unloadedImage.gameObject.SetActive(false);
    }

    public float SetLoadingTime(string rarity)
    {
        if (rarity == "ÀÏ¹Ý")
            return RarityLoadingTime.Common;
        else if (rarity == "°í±Þ")
            return RarityLoadingTime.Uncommon;
        else if (rarity == "Èñ±Í")
            return RarityLoadingTime.Rare;
        else if (rarity == "Àü¼³")
            return RarityLoadingTime.Legendary;
        
        return 0;
    }
}
