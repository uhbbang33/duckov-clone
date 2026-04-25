using System.Collections;
using UnityEngine;

public class FadeController : MonoBehaviour
{
    [SerializeField] private float _fadeInDuration = 2.0f;
    [SerializeField] private float _fadeOutDuration = 2.0f;

    private CanvasGroup _canvasGroup;

    protected virtual void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0f;
    }

    public IEnumerator FadeIn()
    {
        _canvasGroup.blocksRaycasts = true;

        float startAlpha = _canvasGroup.alpha;
        float timer = 0f;
        while (timer < _fadeInDuration)
        {
            timer += Time.unscaledDeltaTime;
            _canvasGroup.alpha = Mathf.Lerp(startAlpha, 1f, timer / _fadeInDuration);
            yield return null;
        }

        _canvasGroup.alpha = 1f;
    }

    public IEnumerator FadeOut()
    {
        float startAlpha = _canvasGroup.alpha;
        float timer = 0f;
        while (timer < _fadeOutDuration)
        {
            timer += Time.unscaledDeltaTime;
            _canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, timer / _fadeOutDuration);
            yield return null;
        }

        _canvasGroup.alpha = 0f;
        _canvasGroup.blocksRaycasts = false;
    }
}
