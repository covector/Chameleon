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
        canvas = GetComponent<Canvas>();
    }
    public void Play()
    {
        Cursor.lockState = CursorLockMode.None;
        FindFirstObjectByType<ProximityCue>().IsInRange(false);
        int itemsCollected = FindFirstObjectByType<ItemCounter>().GetCount();
        failText.text = "SUBJECT ID." + TerrainGeneration.instance.seed.ToString().PadLeft(5, '0') + "\n-<b>FAILED</b>-" + "\nCOLLECTED." + itemsCollected;
        GetComponent<Typing>().HideAll();
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
        TerrainGeneration.instance.ResetPreGen();
    }

    private float counter = 0;
    private void Update()
    {
        if (glowText)
        {
            backText.color = new Color(0.3f, 0.3f, 0.3f, MapValues(Mathf.Cos(counter), -1f, 1f, 1f, 0.2f));
            counter += Time.unscaledDeltaTime * frequency;
        }

        if (canvas.enabled && !uiLock && (
            Input.GetKeyDown(KeyCode.Space) || Input.GetKeyUp(KeyCode.Return) || Input.GetMouseButtonDown(0)
        ))
        {
            if (canExit)
            {
                uiLock = true;
                FindFirstObjectByType<SceneTransition>().FadeOut(callback: () => SceneManager.LoadScene("Title"));
            }
            else
            {
                GetComponent<Typing>().Finish();
            }
        }
    }
}
