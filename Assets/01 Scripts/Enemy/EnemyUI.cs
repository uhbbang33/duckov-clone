using System.Collections;
using UnityEngine;

public class EnemyUI : MonoBehaviour
{
    [SerializeField] private GameObject _targetingIcon;
    [SerializeField] private GameObject _warningIcon;
    [SerializeField] private Renderer _renderer;
    [SerializeField] private GameObject _canvas;

    private WaitForSeconds _waitForShowWarningIcon;

    private const float _showWarningIconDuration = 1f;

    private void Awake()
    {
        _waitForShowWarningIcon = new WaitForSeconds(_showWarningIconDuration);
    }

    public void ShowTargetingIcon(bool show)
    {
        if (show && _warningIcon.activeSelf == false)
            _targetingIcon.SetActive(true);
        else
            _targetingIcon.SetActive(false);
    }

    public void ShowWarningIcon(bool show)
    {
        if (show && _targetingIcon.activeSelf)
            ShowTargetingIcon(false);

        _warningIcon.SetActive(show);
    }

    public void SetVisible(bool show)
    {
        _renderer.enabled = show;
        _canvas.SetActive(show);
    }

    public IEnumerator ShowWarningIconCoroutine()
    {
        ShowWarningIcon(true);
        yield return _waitForShowWarningIcon;
        ShowWarningIcon(false);
    }

}
