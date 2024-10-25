using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RandomAudio : MonoBehaviour
{
    public AudioClip[] audioClips;
    public AudioSource audioSource;

    private void Start()
    {
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
    }

    public void PlayRandomSound(float volume = 1f)
    {
        audioSource.PlayOneShot(GetRandomClip(), volume);
    }

    public AudioClip GetRandomClip()
    {
        return audioClips[Random.Range(0, audioClips.Length)];
    }

    public bool IsPlaying()
    {
        return audioSource.isPlaying;
    }
}
