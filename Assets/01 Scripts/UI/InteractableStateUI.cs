using UnityEngine;
using UnityEngine.UI;

public class InteractableStateUI : MonoBehaviour
{
    [SerializeField] private GameObject _canvas;
    [SerializeField] protected Image _stateImage;
    [SerializeField] private GameObject _infoUI;

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
        _infoUI.SetActive(true);
    }

    public virtual void Deselected()
    {
        _infoUI.SetActive(false);
    }
}
