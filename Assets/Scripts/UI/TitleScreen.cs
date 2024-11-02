using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    public Canvas titleCanvas;
    public Canvas optionCanvas;
    public Canvas creditCanvas;
    public ConfirmBox confirm;

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
        FindFirstObjectByType<SceneTransition>().FadeOut(callback: () => SceneManager.LoadScene("GameScene"));
    }

    public void Options()
    {
        titleCanvas.enabled = false;
        creditCanvas.enabled = false;
        optionCanvas.enabled = true;
        FindFirstObjectByType<UIState>().Push("Page");
    }

    public void Credits()
    {
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
        confirm.Show("Exit Game?", (bool yes) =>
        {
            if (yes) { FindFirstObjectByType<SceneTransition>().FadeOut(callback: () => Application.Quit()); }
        });
    }
}
