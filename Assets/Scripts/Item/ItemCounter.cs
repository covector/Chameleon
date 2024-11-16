using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
public class ItemCounter : MonoBehaviour
{
    private int count = 0;
    public TextMeshProUGUI counter;

    private void Start()
    {
        Display();
    }

    public void Increment()
    {
        count++;
        Display();
    }

    private void Display()
    {
        counter.text = count > 0 ? count.ToString() : "";
    }

    public int GetCount() { return count; }
}
