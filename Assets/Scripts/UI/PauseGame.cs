using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class PauseGame : MonoBehaviour
{
    public static PauseGame instance;
    public bool isFrozen { get; private set; }

    Canvas parentCanvas;
    public Canvas pauseCanvas;
    public Canvas optionCanvas;
    public ConfirmBox confirm;

    void Start()
    {
        parentCanvas = GetComponent<Canvas>();
        instance = this;
        Resume();
    }

    private void ToggleDOF(bool toggle)
    {
        DepthOfField dof;
        FindFirstObjectByType<Volume>().profile.TryGet(out dof);
        dof.active = toggle;
    }

    private void ToggleLensDistort(bool toggle)
    {
        LensDistortion lensDistort;
        FindFirstObjectByType<Volume>().profile.TryGet(out lensDistort);
        lensDistort.active = toggle;
    }

    public void Resume()
    {
        ToggleDOF(true);
        Cursor.lockState = CursorLockMode.Locked;
        parentCanvas.enabled = false;
        FindFirstObjectByType<UIState>().Clear();
        isFrozen = false;
        Time.timeScale = 1.0f;
        AudioListener.pause = false;
    }

    public void Pause()
    {
        ToggleDOF(false);
        Cursor.lockState = CursorLockMode.None;
        parentCanvas.enabled = true;
        isFrozen = true;
        Time.timeScale = 0.0f;
        AudioListener.pause = true;
    }

    public void Options()
    {
        pauseCanvas.enabled = false;
        optionCanvas.enabled = true;
        ToggleLensDistort(false);
        FindFirstObjectByType<UIState>().Push("Page");
    }

    public void Back()
    {
        pauseCanvas.enabled = true;
        optionCanvas.enabled = false;
        ToggleLensDistort(true);
        FindFirstObjectByType<UIState>().Remove("Page");
    }

    public void Quit()
    {
        confirm.Show("Back to Title?", (bool yes) =>
        {
            if (yes) {
                Time.timeScale = 1.0f;
                AudioListener.pause = false;
                SceneManager.LoadScene("Title"); 
            }
        });
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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && FindFirstObjectByType<UIState>().IsEmpty())
        {
            if (isFrozen) { Resume(); } else { Pause(); }
        }
    }
}
