using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ProximityCue : PlayableBlend
{
    public Volume volume;
    private Vignette vignette;
    private ChromaticAberration aberration;
    private bool inRange = false;

    private void Start()
    {
        volume.profile.TryGet(out vignette);
        volume.profile.TryGet(out aberration);
    }

    public override void SetTo(float t)
    {
        vignette.intensity.value = Mathf.Lerp(0f, 0.75f, t);
        aberration.intensity.value = Mathf.Lerp(0f, 0.75f, t);
    }

    public void IsInRange(bool isIn)
    {
        if (inRange != isIn)
        {
            inRange = isIn;
            Play(isIn);
        }
    }
}
