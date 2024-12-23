using System.Collections;
using UnityEngine;
using System;
using UnityEngine.UI;
using static Utils;

[DisallowMultipleComponent]
public class SceneTransition : MonoBehaviour
{
    private Coroutine _coroutine;
    private Image image;

    private void Start()
    {
        image = GetComponent<Image>();
    }

    public void StartAndReplace(IEnumerator coroutine)
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }
        _coroutine = StartCoroutine(coroutine);
    }

    public void FadeIn(float duration = 2f, Action callback = null)
    {
        StartAndReplace(_FadeIn(duration, callback));
    }

    private IEnumerator _FadeIn(float duration, Action callback)
    {
        for (float t = 0; t < duration; t+= Time.unscaledDeltaTime)
        {
            float progress = t / duration;
            UpdateFade(progress);
            yield return null;
        }
        UpdateFade(1f);
        if (callback != null) { callback(); }
    }

    public void FadeOut(float duration = 2f, Action callback = null)
    {
        StartAndReplace(_FadeOut(duration, callback));
    }

    private IEnumerator _FadeOut(float duration, Action callback)
    {
        for (float t = 0; t < duration; t += Time.unscaledDeltaTime)
        {
            float progress = 1 - t / duration;
            UpdateFade(progress);
            yield return null;
        }
        UpdateFade(0f);
        if (callback != null) { callback(); }
    }

    private float PowerCorrect(float x)
    {
        return Mathf.Pow(x, 3f);
    }

    private void UpdateFade(float progress)
    {
        AudioListener.volume = progress * FindFirstObjectByType<PlayerOptions>().Volume;
        image.color = new Color(0, 0, 0, 1 - PowerCorrect(progress));
    }

    public void DelayedFadeIn(float delay = 1f, bool unscaledTime = false)
    {
        UpdateFade(0);
        RunDelay(this, () => FadeIn(), delay, unscaledTime);
    }

    public void BlockScreen()
    {
        image.color = new Color(0, 0, 0, 1);
    }

    public void UnblockScreen()
    {
        image.color = new Color(0, 0, 0, 0);
    }
}
