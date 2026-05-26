using TMPro;
using UnityEngine;

public class DynamicFontSize : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private float _defaultSize;
    [SerializeField] private int _startReduceLength;

    private bool _isRefrsh = false;

    private void OnEnable()
    {
        if (!_isRefrsh)
        {
            if (_text.text != null)
                SetTextSize();

            _isRefrsh = true;
        }
    }

    public void SetTextSize()
    {
        int reduceAmount = _text.text.Length - _startReduceLength;

        if (reduceAmount > 0)
            _text.fontSize = _defaultSize - 10 * reduceAmount;
        else
            _text.fontSize = _defaultSize;
    }
}
