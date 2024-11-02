using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class UIState : MonoBehaviour
{
    private List<string> stateStack = new List<string>();

    public void Remove(string state)
    {
        StartCoroutine(RemoveDelay(state));
    }
    public void _Remove(string state)
    {
        if (stateStack.Contains(state)) { stateStack.Remove(state); }
    }
    public IEnumerator RemoveDelay(string state)
    {
        yield return new WaitForSecondsRealtime(0.2f);
        _Remove(state);
    }

    public void Push(string state)
    {
        _Remove(state);
        stateStack.Add(state);
    }

    public bool IsState(string state)
    {
        if (IsEmpty()) { return false; }
        return state.Equals(stateStack[stateStack.Count - 1]);
    }

    public bool IsEmpty()
    {
        return stateStack.Count == 0;
    }

    public void Clear()
    {
        stateStack.Clear();
    }

    public override string ToString()
    {
        string result = string.Empty;
        foreach (string state in stateStack)
        {
            result += state + ", ";
        }
        return result;
    }
}
