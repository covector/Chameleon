using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RandomAudio : MonoBehaviour
{
    public AudioClip[] audioClips;
    AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayRandomSound(float volume = 1f)
    {
        audioSource.PlayOneShot(GetRandomClip(), volume);
    }

    public void PlayRandomSoundAt(Vector2 position, float volume = 1f)
    {
        float y = ChunkGeneration.GetGroudLevel(position.x, position.y) + 1f;
        AudioSource.PlayClipAtPoint(GetRandomClip(), new Vector3(position.x, y, position.y), volume);
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
