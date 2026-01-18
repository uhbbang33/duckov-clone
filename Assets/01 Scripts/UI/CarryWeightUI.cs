using TMPro;
using UnityEngine;

public class CarryWeightUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _weightText;


    private void Awake()
    {
        GameManager.Instance.Inventory.OnWeightChange += ChangeWeightText;

    }

    private void ChangeWeightText(float current, float max)
    {
        _weightText.text = current.ToString() + "/" + max.ToString() + "kg";
    }
}
