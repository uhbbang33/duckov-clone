using System.Collections;
using UnityEngine;

public class EnemyUI : MonoBehaviour
{
    [SerializeField] private GameObject _findingIcon;
    [SerializeField] private GameObject _warningIcon;
    [SerializeField] private Renderer _renderer;
    [SerializeField] private GameObject _canvas;

    private WaitForSeconds _waitForShowWarningIcon;

    private const float _showWarningIconDuration = 1f;

    private void Awake()
    {
        _waitForShowWarningIcon = new WaitForSeconds(_showWarningIconDuration);
    }

    public void ShowFindingIcon(bool show)
    {
        if (show && _warningIcon.activeSelf == false)
            _findingIcon.SetActive(true);
        else
            _findingIcon.SetActive(false);
    }

    public void ShowWarningIcon(bool show)
    {
        if (show && _findingIcon.activeSelf)
            ShowFindingIcon(false);

        _warningIcon.SetActive(show);
    }

    public void SetCanvasVisible(bool show)
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
