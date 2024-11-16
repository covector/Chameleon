using UnityEngine;

public class TitleScreen : MonoBehaviour
{
    private bool uiLock = false;
    public Canvas titleCanvas;
    public Canvas optionCanvas;
    public Canvas creditCanvas;
    public ConfirmBox confirm;
    public ModeSelection modeSelection;

    private void Start()
    {
        FindFirstObjectByType<SceneTransition>().DelayedFadeIn();
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 1.0f;
        AudioListener.pause = false;
        Back();
    }

    public void Play()
    {
        if (uiLock) { return; }
        modeSelection.Show(() =>
        {
            uiLock = true;
        });
    }

    public void Options()
    {
        if (uiLock) { return; }
        titleCanvas.enabled = false;
        creditCanvas.enabled = false;
        optionCanvas.enabled = true;
        FindFirstObjectByType<UIState>().Push("Page");
    }

    public void Credits()
    {
        if (uiLock) { return; }
        titleCanvas.enabled = false;
        creditCanvas.enabled = true;
        optionCanvas.enabled = false;
        FindFirstObjectByType<UIState>().Push("Page");
    }

    public void Back()
    {
        titleCanvas.enabled = true;
        creditCanvas.enabled = false;
        optionCanvas.enabled = false;
        FindFirstObjectByType<UIState>().Remove("Page");
    }

    void OnGUI()
    {
        if (
            (Event.current.isKey && Event.current.Equals(Event.KeyboardEvent("escape"))) &&
            FindFirstObjectByType<UIState>().IsState("Page")
        )
        {
            Back();
        }
    }

    public void Quit()
    {
        if (uiLock) { return; }
        confirm.Show("Exit Game?", (bool yes) =>
        {
            if (yes) { uiLock = true; FindFirstObjectByType<SceneTransition>().FadeOut(callback: () => Application.Quit()); }
        });
    }
}
