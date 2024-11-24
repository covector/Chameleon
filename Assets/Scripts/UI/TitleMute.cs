using UnityEngine;
using UnityEngine.UI;

public class TitleMute : MonoBehaviour
{
    public Image image;
    public LoopAudio muteAudio;
    public Color activeColor;
    public Color inactiveColor;
    private static bool mute = false;

    private void Start()
    {
        Set(mute);
    }

    private void Set(bool mute)
    {
        image.color = mute ? inactiveColor : activeColor;
        muteAudio.SetVolume(mute ? 0 : 1);
    }

    public void ToggleMute()
    {
        mute = !mute;
        Set(mute);
    }
}
