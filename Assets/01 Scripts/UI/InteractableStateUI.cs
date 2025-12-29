using UnityEngine;
using UnityEngine.UI;

public class InteractableStateUI : MonoBehaviour
{
    [SerializeField] private GameObject _canvas;
    [SerializeField] protected Image _stateImage;
    [SerializeField] private GameObject _infoUI;

    private PlayerInteract _interact;

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

        _stateImage.sprite = GameResources.Instance.OpenableBoxSprite;

        // PlayerInteract에 넘겨주기
        _interact.UI = this;
    }

    public virtual void Deselected()
    {
        _infoUI.SetActive(false);

        _stateImage.sprite = GameResources.Instance.UnopenedBoxSprite;

        // PlayerInteract에서 빠지기
        _interact.UI = null;
    }
}
