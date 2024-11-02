using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class KeybindOption : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public TextMeshProUGUI assigned;
    private bool isEditing;
    public KeyCode assignedKey;
    public Action<KeyCode> OnChangeCallback;

    public void SetUp(Action<KeyCode> OnChangeCallback, KeyCode defaultKeyCode)
    {
        this.OnChangeCallback = OnChangeCallback;
        assignedKey = defaultKeyCode;
        assigned.text = ToReadable(assignedKey);
    }

    private string ToReadable(KeyCode keyCode)
    {
        if ((int)keyCode >= (int)KeyCode.Alpha0 && (int)keyCode <= (int)KeyCode.Alpha9) { return ((int)keyCode - (int)KeyCode.Alpha0).ToString(); }
        return keyCode.ToString();
    }

    public void OnSelect(BaseEventData eventData)
    {
        isEditing = true;
        assigned.text = "-";
        FindFirstObjectByType<UIState>().Push("Keybind");
    }

    public void OnDeselect(BaseEventData eventData)
    {
        isEditing = true;
        assigned.text = ToReadable(assignedKey);
    }

    private void StopEditing()
    {
        EventSystem.current.SetSelectedGameObject(null);
        isEditing = false;
        FindFirstObjectByType<UIState>().Remove("Keybind");
    }

    void OnGUI()
    {
        if (isEditing && Event.current.isKey && Event.current.keyCode != KeyCode.None)
        {
            if (!Event.current.Equals(Event.KeyboardEvent("escape")) && FindFirstObjectByType<UIState>().IsState("Keybind"))
            {
                assignedKey = Event.current.keyCode;
                OnChangeCallback(assignedKey);
            }
            StopEditing();
        }
    }
}
