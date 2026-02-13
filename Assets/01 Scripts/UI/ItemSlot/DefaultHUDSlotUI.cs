using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DefaultHUDSlotUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _countText;
    [SerializeField] protected Image _iconImage;
    [SerializeField] private GameObject _durabilityUI;
    [SerializeField] private GameObject _countUI;
    [SerializeField] private Slider _durabilitySlider;

    private UIManager _uiManager;
    private RectTransform _rect;

    private void Start()
    {
        _uiManager = UIManager.Instance;
        _uiManager.ChangeImageAlpha(_iconImage, false);
    }

    public void RefresuhDefaultUHD(bool isShow, string nameText = null, string countText = null, Sprite iconImage = null, bool activateDurabilityUI = false, bool activateCountUI = false, float durabilityValue = 0f)
    {
        _nameText.text = nameText;
        _countText.text = countText;
        _iconImage.sprite = iconImage;
        _durabilitySlider.value = durabilityValue;

        _durabilityUI.SetActive(activateDurabilityUI);
        _countUI.SetActive(activateCountUI);

        _uiManager.ChangeImageAlpha(_iconImage, isShow);

        RebuildLayout();
    }

    public void RebuildLayout()
    {
        if (_rect == null)
            _rect = GetComponent<RectTransform>();
        LayoutRebuilder.ForceRebuildLayoutImmediate(_rect);
    }
}
