using System.Collections;
using UnityEngine;

public class LoopAudio : MonoBehaviour
{
    public AudioClip audioClip;
    AudioSource audioSource1;
    AudioSource audioSource2;
    public float volume = 0.1f;
    private float finalVolume = 0.1f;
    public float blendTime = 3.0f;
    private bool audioSource1Main = true;
    private bool playing = false;
    private bool fading = false;
    public bool playOnAwake = true;
    void Awake()
    {
        audioSource1 = gameObject.AddComponent<AudioSource>();
        audioSource2 = gameObject.AddComponent<AudioSource>();
        audioSource1.clip = audioClip;
        audioSource2.clip = audioClip;
        audioSource1.playOnAwake = false;
        audioSource2.playOnAwake = false;
        finalVolume = volume;
        if (playOnAwake)
        {
            Play();
        }
    }

    private AudioSource GetMainSource()
    {
        return audioSource1Main ? audioSource1 : audioSource2;
    }

    IEnumerator ScheduleCheck()
    {
        if (playing)
        {
            yield return new WaitForSeconds(blendTime);
            float remain = audioClip.length - GetMainSource().time - blendTime;
            if (remain < blendTime)
            {
                StartCoroutine(CrossFade(audioSource1Main, remain));
                audioSource1Main = !audioSource1Main;
            }
            StartCoroutine(ScheduleCheck());
        } else
        {
            yield return null;
        }
    }

    private float logTrans(float x)
    {
        return Mathf.Log(x + 1f, 2);
    }

    private float polyTrans(float x)
    {
        return Mathf.Pow(x, 0.5f);
    }

    IEnumerator CrossFade(bool source1, float delay)
    {
        yield return new WaitForSeconds(delay);
        fading = true;
        if (source1)
        {
            audioSource2.Play();
        } else
        {
            audioSource1.Play();
        }
        for (float t = 0; t < blendTime; t += Time.deltaTime)
        {
            float progressIn = polyTrans(t / blendTime);
            float progressOut = polyTrans(1f - t / blendTime);
            audioSource1.volume = finalVolume * (source1 ? progressOut : progressIn);
            audioSource2.volume = finalVolume * (source1 ? progressIn : progressOut);
            yield return null;
        }
        audioSource1.volume = source1 ? 0f : finalVolume;
        audioSource2.volume = source1 ? finalVolume : 0f;
        fading = false;
    }

    public void SetVolume(float volumeMultiplier)
    {
        finalVolume = volume * volumeMultiplier;
        if (!fading)
        {
            audioSource1.volume = finalVolume;
            audioSource2.volume = finalVolume;
        }
    }

    public void Play()
    {
        audioSource1.volume = finalVolume;
        audioSource2.volume = 0;
        audioSource1.Play();
        playing = true;
        StartCoroutine(ScheduleCheck());
    }

    public void Stop()
    {
        audioSource1.Stop();
        audioSource2.Stop();
        playing = false;
    }
}
