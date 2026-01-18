using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CarryWeightUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _weightText;
    [SerializeField] private Slider _weightSlider;


    private void Awake()
    {
        GameManager.Instance.Inventory.OnWeightChange += ChangeWeightText;

    }

    private void ChangeWeightText(float current, float max)
    {
        _weightText.text = current.ToString() + "/" + max.ToString() + "kg";

        _weightSlider.value = current / max;
    }
}
