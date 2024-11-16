using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ModeSelection : MonoBehaviour
{
    private Canvas canvas;
    public RectTransform boundary;
    private Action currentCallback = null;
    private bool uiLock = false;

    void Start()
    {
        canvas = GetComponent<Canvas>();
        canvas.enabled = false;
    }

    public void Show(Action callback)
    {
        if (canvas.enabled) { return; }
        canvas.enabled = true;
        currentCallback = callback;
        FindFirstObjectByType<UIState>().Push("Mode");
    }

    public void Hide()
    {
        canvas.enabled = false;
        FindFirstObjectByType<UIState>().Remove("Mode");
    }

    public void PlayNormally()
    {
        if (uiLock) { return; }
        GameSettings.includeTutorial = false;
        GameSettings.includeMonster = true;
        LoadGameScene();
    }

    public void PlayWithTutorial()
    {
        if (uiLock) { return; }
        GameSettings.includeTutorial = true;
        GameSettings.includeMonster = true;
        LoadGameScene();
    }

    public void PlayWithoutMonster()
    {
        if (uiLock) { return; }
        GameSettings.includeTutorial = false;
        GameSettings.includeMonster = false;
        LoadGameScene();
    }

    private void LoadGameScene()
    {
        uiLock = true;
        if (currentCallback != null) { currentCallback(); }
        FindFirstObjectByType<SceneTransition>().FadeOut(callback: () => SceneManager.LoadScene("GameScene"));
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
            FindFirstObjectByType<UIState>().IsState("Mode")
        )
        {
            Hide();
        }
    }
}
