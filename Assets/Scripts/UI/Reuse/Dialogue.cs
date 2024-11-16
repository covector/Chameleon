using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Typing))]
public class Dialogue : MonoBehaviour
{
    public GameObject dialogueBox;
    public TextMeshProUGUI dialogueText;
    public Button nextButton;
    private Action currentCallback = null;
    public float secBetweenWords = 0.1f;

    void Start()
    {
        dialogueBox.SetActive(false);
        nextButton.onClick.AddListener(() => { OnClickNext(); });
        nextButton.gameObject.SetActive(false);
        dialogueText.text = string.Empty;
    }

    public void Say(string sentence, Action callback)
    {
        if (currentCallback != null) { return; }
        currentCallback = callback;
        dialogueText.text = sentence;
        GetComponent<Typing>().HideAll();
        dialogueBox.SetActive(true);
        nextButton.gameObject.SetActive(false);
        Utils.RunDelay(this, () =>
        {
            GetComponent<Typing>().Play(secBetweenWords, () =>
            {
                Cursor.lockState = CursorLockMode.None;
                nextButton.gameObject.SetActive(true);
            });
        }, 1f);
    }

    private void OnClickNext()
    {
        if (nextButton.gameObject.activeSelf) 
        { 
            Cursor.lockState = CursorLockMode.Locked;
            Action temp = currentCallback;
            currentCallback = null;
            Utils.RunDelay(temp, 0.02f);
        }
    }

    private void Update()
    {
        if (currentCallback != null && (
            Input.GetKeyDown(KeyCode.Space) || Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.RightArrow)
        ))
        {
            OnClickNext();
        }
    }

    public void HideAll()
    {
        dialogueBox.SetActive(false);
        nextButton.gameObject.SetActive(false);
        dialogueText.text = string.Empty;
    }

    public bool CursorIsLocked()
    {
        return !nextButton.gameObject.activeSelf;
    }
}
