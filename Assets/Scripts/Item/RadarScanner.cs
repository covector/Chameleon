using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Animator))]
public class RadarScanner : MonoBehaviour
{
    Animator animator;
    public RadarDisplay display;
    public bool isEnabled = false;
    public Volume volume;
    public AudioSource switchOnSound;
    LoopAudio loop;
    DepthOfField dof;
    float t = 0;
    void Start()
    {
        animator = GetComponent<Animator>();
        volume.profile.TryGet(out dof);
        loop = GetComponent<LoopAudio>();
    }

    void Update()
    {
        if (Input.GetKeyDown(PlayerOptions.instance.KeyBinds["Scanner"]) && !FindFirstObjectByType<PauseGame>().pauseLock)
        {
            if (!isEnabled)
            {
                TurnOnRadar();
            } else
            {
                TurnOffRadar();
            }
            if (Tutorial.waitingScanner) { Tutorial.Scanner(); }
        }
        if (!Tutorial.inProgress)
        {
            if (isEnabled ? t > 0f : t < 1f)
            {
                t += isEnabled ? -Time.deltaTime : Time.deltaTime;
                dof.focusDistance.value = Mathf.Lerp(0.7f, 5f, t);
            }
            if (isEnabled ? t < 0f : t > 1f)
            {
                t = isEnabled ? 0f : 1f;
            }
        }
    }

    public void TurnOnRadar()
    {
        if (isEnabled) { return; }
        isEnabled = true;
        animator.ResetTrigger("RadarOut");
        animator.SetTrigger("RadarOut");
        switchOnSound.Play();
    }

    public void TurnOffRadar()
    {
        if (!isEnabled) { return; }
        isEnabled = false;
        animator.ResetTrigger("RadarIn");
        animator.SetTrigger("RadarIn");
    }

    public void MaterialScreenOn()
    {
        display.MaterialScreenOn();

        loop.Play();
    }

    public void MaterialScreenOff()
    {
        display.MaterialScreenOff();
        loop.Stop();
    }
}
