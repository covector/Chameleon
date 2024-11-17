using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using static Utils;

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

    public void OnSelect(BaseEventData eventData)
    {
        Debug.Log("onselect");
        isEditing = true;
        assigned.text = "-";
        FindFirstObjectByType<UIState>().Push("Keybind");
    }

    public void OnDeselect(BaseEventData eventData)
    {
        Debug.Log("ondeselect");
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
                if (FindFirstObjectByType<PlayerOptions>().canEdit)
                {
                    assignedKey = Event.current.keyCode;
                }
                OnChangeCallback(assignedKey);
            }
            StopEditing();
        }
    }
}
