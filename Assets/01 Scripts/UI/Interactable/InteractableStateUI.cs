using UnityEngine;
using UnityEngine.UI;

public class InteractableStateUI : MonoBehaviour
{
    [SerializeField] private GameObject _canvas;
    [SerializeField] protected Image _stateImage;
    [SerializeField] private GameObject _infoUI;
    [SerializeField] private InteractableType _type;

    private PlayerInteract _interact;

    public InteractableType Type
    {
        get { return _type; }
    }

    private void Start()
    {
        _interact = GameManager.Instance.PlayerObject.GetComponent<PlayerInteract>();
    }

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

        _interact.UI = this;
    }

    public virtual void Deselected()
    {
        _infoUI.SetActive(false);

        _interact.UI = null;
    }

    public virtual void OnInteract() { }
}
