using System.Collections;
using UnityEngine;

public class RandomLocationSound : MonoBehaviour
{
    public Transform monster;
    MonsterStateMachine stateMachine;
    public Transform player;
    RandomAudio randomAudio;
    AudioSource audioSource;
    private bool playing = false;
    private float theta = 0.0f;
    private float angularSpeed = 1.0f;
    private bool fading = false;
    private Updater updater;

    private void Start()
    {
        randomAudio = GetComponent<RandomAudio>();
        stateMachine = monster.GetComponent<MonsterStateMachine>();
        audioSource = GetComponent<AudioSource>();
        updater = new Updater(Random.Range(15f, 30f), TryPlaySound);
    }

    private void Update()
    {
        if (playing)
        {
            theta += angularSpeed * Time.deltaTime;
            Vector2 loc = new Vector2(player.position.x + 10f * Mathf.Sin(theta), player.position.z + 10f * Mathf.Cos(theta));
            transform.position = Utils.ProjectToGround(loc);
            if (!randomAudio.IsPlaying()) { playing = false; }
            if (!fading && !CanPlaySound())
            {
                fading = true;
                StartCoroutine(FadeOutAudio());
            }
        }
        updater.Update();
    }

    void TryPlaySound()
    {
        if (!playing && CanPlaySound())
        {
            PlayRandomSound();
        }
        updater.period = Random.Range(15f, 30f);
    }

    IEnumerator FadeOutAudio()
    {
        for (float v = audioSource.volume; v > 0; v -= Time.deltaTime)
        {
            audioSource.volume = v;
            yield return null;
        }
    }

    void PlayRandomSound()
    {
        fading = false;
        audioSource.volume = 1f;
        theta = Random.Range(0f, 2f * Mathf.PI);
        float volume = Random.Range(0.3f, 1f);
        angularSpeed = Random.Range(0.5f, 1.5f);
        playing = true;
        randomAudio.PlayRandomSound(volume);
    }

    bool CanPlaySound()
    {
        return stateMachine.current == MonsterStateMachine.State.Approach && (monster.position - player.position).sqrMagnitude > 100f;
    }
}
