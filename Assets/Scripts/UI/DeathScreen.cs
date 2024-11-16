using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Utils;

public class DeathScreen : MonoBehaviour
{
    Canvas canvas;
    private bool glowText;
    public float frequency;
    private bool uiLock;
    public TextMeshProUGUI failText;
    public TextMeshProUGUI backText;
    public AudioClip beep;
    private bool canExit;
    private void Start()
    {
        glowText = false;
        canExit = false;
        uiLock = false;
    }
    public void Play()
    {
        Cursor.lockState = CursorLockMode.None;
        int itemsCollected = FindFirstObjectByType<ItemCounter>().GetCount();
        failText.text = "SUBJECT ID." + TerrainGeneration.instance.seed.ToString().PadLeft(5, '0') + "\n-<b>FAILED</b>-" + "\nCOLLECTED." + itemsCollected;
        GetComponent<Typing>().HideAll();
        canvas = GetComponent<Canvas>();
        canvas.enabled = true;
        GetComponent<AudioSource>().PlayOneShot(beep);
        RunDelay(this, () =>
        {
            GetComponent<Typing>().Play(0.1f, () =>
            {
                canExit = true;
                RunDelay(this, () =>
                {
                    backText.enabled = true;
                    glowText = true;
                }, 4f);
            });
        }, 3f);
    }

    private float counter = 0;
    private void Update()
    {
        if (glowText)
        {
            backText.color = new Color(0.3f, 0.3f, 0.3f, MapValues(Mathf.Cos(counter), -1f, 1f, 1f, 0.2f));
            counter += Time.deltaTime * frequency;
        }
    }

    private void OnGUI()
    {
        if (!canExit || canvas.enabled == false || uiLock) return;
        if (
            ((Event.current.isMouse) ||
            (Event.current.isKey && Event.current.Equals(Event.KeyboardEvent("return"))))
        )
        {
            uiLock = true;
            FindFirstObjectByType<SceneTransition>().FadeOut(callback: () => SceneManager.LoadScene("Title"));
        }
    }
}
