using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSplitUI : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private TextMeshProUGUI _countText;
    [SerializeField] private Button _splitButton;

    private ItemSlot _currentSlot;

    public ItemSlot CurrentSlot
    {
        get { return _currentSlot; }
        set { _currentSlot = value; }
    }

    private void OnEnable()
    {
        if (_currentSlot.Quantity < 2)
            Debug.LogError("The Slot Quantity must be at least 2");

        _slider.maxValue = _currentSlot.Quantity - 1;
    }

    // On Slider Value Changed
    public void ChangeValueText()
    {
        _countText.text = _slider.value.ToString();
    }

    // On Split Button Click
    public void SplitItem()
    {
        _currentSlot.Quantity -= (int)_slider.value;
        _currentSlot.UI.RefreshUI();
        _currentSlot.SplitItem((int)_slider.value);

        gameObject.SetActive(false);
    }

}
