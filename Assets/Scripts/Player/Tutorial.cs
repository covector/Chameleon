using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public bool inProgress { get; private set; }

    private void Start()
    {
        inProgress = false;
    }

    public void PlayTutorialDelay(float delay)
    {
        inProgress = true;
        Utils.RunDelay(this, () => PlayTutorial(), delay);
    }

    public void PlayTutorial()
    {
        inProgress = true;
        Dialogue d = FindFirstObjectByType<Dialogue>();
        d.Say("sentence 1 test test,sdffjsd", () =>
        d.Say("sentence 2 test test,sdffjsd", () =>
        d.Say("sentence 3 test test,sdffjsd", () =>
        d.Say("sentence 4 test test,sdffjsd", () =>
        { d.HideAll(); inProgress = false; }
        ))));  // Callback hell lol
    }
}
