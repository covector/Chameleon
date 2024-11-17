using UnityEngine;
using static DeathCoordinator;

public class TutorialDialogue : Dialogue
{
    public AudioClip endSound;
    public AudioClip[] randomSpeaking;
    public AudioClip glitch1;
    public AudioClip glitch2;
    public AudioSource audioSource;
    public bool useGlitch1 { get; set; }
    public bool useGlitch2 { get; set; }
    public override void OnTyping() {
        if (!audioSource.isPlaying)
        {
            if (useGlitch2)
            {
                audioSource.clip = glitch2;
            }
            else if (useGlitch1)
            {
                audioSource.clip = glitch1;
            }
            else
            {
                audioSource.clip = randomSpeaking[Random.Range(0, randomSpeaking.Length)];
            }
            audioSource.Play();
        }
    }
    public override void OnFinishTyping() {

        //audioSource.Stop();
        audioSource.clip = endSound;
        audioSource.Play();
    }
    public override void Start()
    {
        useGlitch1 = false;
        useGlitch2 = false;
        base.Start();
    }
}
