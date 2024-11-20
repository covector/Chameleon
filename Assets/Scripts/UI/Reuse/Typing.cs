using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class Typing : MonoBehaviour
{
    public TextMeshProUGUI text;
    public AudioClip typingSound;
    public float volume = 1f;
    public RandomAudio randomAudio;
    private AudioSource audioSource;
    private Coroutine currentCoroutine = null;
    private Action callback = null;

    void Start()
    {
        if (GetComponent<AudioSource>() != null)
        {
            audioSource = gameObject.GetComponent<AudioSource>();
        }
        if (typingSound != null && GetComponent<AudioSource>() == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    IEnumerator TypingSequence(float waitSeconds, Action callback = null, Action onType = null)
    {
        int totalChar = text.GetParsedText().Length;
        this.callback = callback;
        for (int i = 0; i <= totalChar; i++)
        {
            text.maxVisibleCharacters = i;
            if (randomAudio != null)
            {
                randomAudio.PlayRandomSound();
            } else if (typingSound != null)
            {
                audioSource.PlayOneShot(typingSound, volume);
            }
            if (onType != null) { onType(); }
            yield return new WaitForSeconds(waitSeconds);
        }
        currentCoroutine = null;
        if (callback != null) { callback(); }
    }

    public void Play(float waitSeconds, Action callback = null, Action onType = null)
    {
        if (currentCoroutine == null) {
            currentCoroutine = StartCoroutine(TypingSequence(waitSeconds, callback, onType));
        }
    }

    public void Stop()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }
    }

    public void HideAll()
    {
        text.maxVisibleCharacters = 0;
    }

    public void ShowAll()
    {
        text.maxVisibleCharacters = text.text.Length;
    }

    public void Finish()
    {
        if (currentCoroutine != null)
        {
            Stop();
            ShowAll();
            if (callback != null) { callback(); }
        }
    }
}
