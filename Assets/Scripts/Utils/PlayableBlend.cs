using System.Collections;
using UnityEngine;

public abstract class PlayableBlend : MonoBehaviour
{
    public float duration = 1f;

    IEnumerator _Animate(bool forward)
    {
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            SetTo((forward ? t : (1f - t)) / duration);
            yield return null;
        }
    }

    public void PlayForward()
    {
        Play(true);
    }

    public void PlayBackward()
    {
        Play(false);
    }

    public void Play(bool forward) 
    {
        StartCoroutine(_Animate(forward));
    }

    public abstract void SetTo(float t);
}
