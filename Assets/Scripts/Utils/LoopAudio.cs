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
    bool audioSource1Main = true;
    bool playing = false;
    public bool playOnAwake = true;
    void Start()
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

    IEnumerator CrossFade(bool source1, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (source1)
        {
            audioSource2.Play();
        } else
        {
            audioSource1.Play();
        }
        for (float t = 0; t < blendTime; t += Time.deltaTime)
        {
            float progress = finalVolume * t / blendTime;
            audioSource1.volume = source1 ? finalVolume - progress : progress;
            audioSource2.volume = source1 ? progress : finalVolume - progress;
            yield return null;
        }
    }

    public void SetVolume(float volume)
    {
        audioSource1.volume = volume;
        audioSource2.volume = volume;
        finalVolume = volume;
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
