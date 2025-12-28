using UnityEngine;
using UnityEngine.UI;

public class InteractableStateUI : MonoBehaviour
{
    [SerializeField] private GameObject _canvas;
    [SerializeField] protected Image _stateImage;

    public void ShowCanvas()
    {
        _canvas.SetActive(true);
    }

    public void HideCanvas()
    {
        _canvas.SetActive(false);
    }

    public virtual void Selected()
    {
    }

    public virtual void Deselected()
    {
    }
}
