using System.Collections;
using UnityEngine;

public class AnimatedTexture : MonoBehaviour
{
    public Renderer meshRenderer;
    public int materialIndex;
    public float duration = 1f;
    public bool reversed = false;

    IEnumerator _Animate()
    {
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            SetTo((reversed ? (1f-t) : t) / duration);
            yield return null;
        }
    }

    public void Play()
    {
        StartCoroutine(_Animate());
    }

    public void SetTo(float t)
    {
        meshRenderer.materials[materialIndex].SetFloat("_Blend", t);
    }
}
