using System;
using TMPro;
using UnityEngine;

public class ConfirmBox : MonoBehaviour
{
    private Canvas canvas;
    public RectTransform boundary;
    public TextMeshProUGUI title;
    private Action<bool> currentCallback = null;

    void Start()
    {
        canvas = GetComponent<Canvas>();
        canvas.enabled = false;
    }

    public bool Show(string message, Action<bool> callback, bool forceOverride=true)
    {
        if (!forceOverride && canvas.enabled) { return false; }
        title.text = message;
        currentCallback = callback;
        canvas.enabled = true;
        FindFirstObjectByType<UIState>().Push("Confirm");
        return true;
    }

    public void Hide()
    {
        currentCallback = null;
        canvas.enabled = false;
        FindFirstObjectByType<UIState>().Remove("Confirm");
    }

    public void OnPress(bool yes)
    {
        currentCallback(yes);
        Hide();
    }

    private bool IsInBox(Vector2 mousePos)
    {
        Vector3[] v = new Vector3[4];
        boundary.GetWorldCorners(v);
        Vector2 bottomLeft = Camera.main.WorldToScreenPoint(v[0]);
        Vector2 topRight = Camera.main.WorldToScreenPoint(v[2]);
        return mousePos.x > bottomLeft.x && mousePos.y > bottomLeft.y && mousePos.x < topRight.x && mousePos.y < topRight.y;
    }

    void OnGUI()
    {
        if (canvas.enabled == false) return;
        if (
            ((Event.current.isMouse && !IsInBox(Event.current.mousePosition)) ||
            (Event.current.isKey && Event.current.Equals(Event.KeyboardEvent("escape")))) &&
            FindFirstObjectByType<UIState>().IsState("Confirm")
        )
        {
            Hide();
        }
    }
}
