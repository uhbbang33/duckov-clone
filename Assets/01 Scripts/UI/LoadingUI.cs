using System.Collections;
using UnityEngine;

public class LoadingUI : SingletonMonoBehaviour<LoadingUI>
{
    [SerializeField] private float _fadeDuration;

    private CanvasGroup _canvasGroup;

    protected override void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(gameObject);

        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0f;
    }

    public IEnumerator FadeIn()
    {
        _canvasGroup.blocksRaycasts = true;

        float startAlpha = _canvasGroup.alpha;
        float timer = 0f;
        while(timer < _fadeDuration)
        {
            timer += Time.unscaledDeltaTime; 
            _canvasGroup.alpha = Mathf.Lerp(startAlpha, 1f, timer / _fadeDuration);
            yield return null;
        }

        _canvasGroup.alpha = 1f;
    }


    public IEnumerator FadeOut()
    {
        float startAlpha = _canvasGroup.alpha;
        float timer = 0f;
        while (timer < _fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            _canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, timer / _fadeDuration);
            yield return null;
        }

        _canvasGroup.alpha = 0f;
        _canvasGroup.blocksRaycasts = false;
    }
}
