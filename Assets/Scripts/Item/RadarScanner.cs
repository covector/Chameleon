using UnityEngine;

[RequireComponent(typeof(Animator))]
public class RadarScanner : MonoBehaviour
{
    Animator animator;
    public RadarDisplay display;
    public bool isEnabled = false;
    void Start()
    {
        animator = GetComponent<Animator>();
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
    }

    void TurnOnRadar()
    {
        animator.ResetTrigger("RadarOut");
        animator.SetTrigger("RadarOut");
    }

    void TurnOffRadar()
    {
        animator.ResetTrigger("RadarIn");
        animator.SetTrigger("RadarIn");
    }

    public void MaterialScreenOn()
    {
        display.MaterialScreenOn();
    }

    public void MaterialScreenOff()
    {
        display.MaterialScreenOff();
    }
}
