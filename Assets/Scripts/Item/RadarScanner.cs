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
        if (Input.GetKeyDown("q"))
        {
            if (!isEnabled)
            {
                TurnOnRadar();
                isEnabled = true;
            } else
            {
                TurnOffRadar();
                isEnabled = false;
            }
        }
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

    void TurnOnRadar()
    {
        animator.ResetTrigger("RadarOut");
        animator.SetTrigger("RadarOut");
        switchOnSound.Play();
    }

    void TurnOffRadar()
    {
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
