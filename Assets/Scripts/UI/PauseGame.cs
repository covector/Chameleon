using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseGame : MonoBehaviour
{
    public static PauseGame instance;
    private bool uiLock = false;
    public bool isFrozen { get; private set; }
    public bool pauseLock { get; private set; }
    public bool canPause { get; set; }

    public Canvas pauseCanvas;
    public Canvas optionCanvas;
    public ConfirmBox confirm;

    void Start()
    {
        FindFirstObjectByType<SceneTransition>().DelayedFadeIn();
        instance = this;
        Resume();
        canPause = true;
    }

    private void ToggleEffects(bool toggle)
    {
        DepthOfField dof;
        FindFirstObjectByType<Volume>().profile.TryGet(out dof);
        dof.active = toggle;

        LensDistortion lensDistort;
        FindFirstObjectByType<Volume>().profile.TryGet(out lensDistort);
        lensDistort.active = toggle;

        ChromaticAberration aberration;
        FindFirstObjectByType<Volume>().profile.TryGet(out aberration);
        aberration.active = toggle;
    }

    public void Resume()
    {
        if (uiLock) { return; }
        ToggleEffects(true);
        //Cursor.lockState = FindFirstObjectByType<Dialogue>().CursorIsLocked() ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.lockState = CursorLockMode.Locked;
        pauseCanvas.enabled = false;
        FindFirstObjectByType<UIState>().Clear();
        isFrozen = false;
        Time.timeScale = 1.0f;
        AudioListener.pause = false;
        StartCoroutine(PauseUnlock());
    }

    public void Pause()
    {
        if (!canPause) { return; }
        ToggleEffects(false);
        Cursor.lockState = CursorLockMode.None;
        pauseCanvas.enabled = true;
        isFrozen = true;
        Time.timeScale = 0.0f;
        AudioListener.pause = true;
        pauseLock = true;
    }

    private IEnumerator PauseUnlock()
    {
        yield return new WaitForFixedUpdate();
        pauseLock = false;
    }

    public void Options()
    {
        if (uiLock) { return; }
        pauseCanvas.enabled = false;
        optionCanvas.GetComponent<PlayerOptions>().Open();
        FindFirstObjectByType<UIState>().Push("Page");
    }

    public void Back()
    {
        pauseCanvas.enabled = true;
        optionCanvas.GetComponent<PlayerOptions>().Close();
        FindFirstObjectByType<UIState>().Remove("Page");
    }

    public void Quit()
    {
        if (uiLock) { return; }
        confirm.Show("Back to Title?", (bool yes) =>
        {
            if (yes) {
                uiLock = true;
                TerrainGeneration.instance.ResetPreGen();
                FindFirstObjectByType<SceneTransition>().FadeOut(callback:() => SceneManager.LoadScene("Title")); 
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
