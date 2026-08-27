using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverSound : MonoBehaviour, IPointerEnterHandler
{
    private SoundManager _soundManager;

    private void Start()
    {
        _soundManager = SoundManager.Instance;
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        _soundManager.PlaySFXOneShot(SFXName.MenuHover);
    }
}
