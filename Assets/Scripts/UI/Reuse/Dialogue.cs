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
    protected Action currentCallback = null;
    public float secBetweenWords = 0.1f;
    public float delayBeforeType = 0.4f;
    public

    virtual void Start()
    {
        dialogueBox.SetActive(false);
        nextButton.onClick.AddListener(() => { OnClickNext(); });
        nextButton.gameObject.SetActive(false);
        dialogueText.text = string.Empty;
    }

    public void Say(string sentence, Action callback = null, bool manualNext = false)
    {
        if (currentCallback != null) { return; }
        currentCallback = callback == null ? () => { } : callback;
        dialogueText.text = sentence;
        GetComponent<Typing>().HideAll();
        dialogueBox.SetActive(true);
        nextButton.gameObject.SetActive(false);
        Utils.RunDelay(this, () =>
        {
            OnStartTyping();
            GetComponent<Typing>().Play(secBetweenWords, () =>
            {
                if (!manualNext)
                {
                    Cursor.lockState = CursorLockMode.None; 
                    nextButton.gameObject.SetActive(true);
                } else { CallCallback(); }
                OnFinishTyping();
            }, onType: OnTyping);
        }, delayBeforeType);
    }

    public void ManualNext()
    {
        OnClickNext();
    }

    private void OnClickNext()
    {
        if (nextButton.gameObject.activeSelf) 
        {
            CallCallback();
        }
    }

    private void CallCallback()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Action temp = currentCallback;
        currentCallback = null;
        Utils.RunDelay(temp, 0.02f);
    }

    public virtual void OnStartTyping() { }
    public virtual void OnTyping() { }
    public virtual void OnFinishTyping() {}

    private void Update()
    {
        //if (currentCallback != null && (
        //    Input.GetKeyDown(KeyCode.Space) || Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.RightArrow)
        //))
        //{
        //    OnClickNext();
        //}
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
