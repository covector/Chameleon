using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSound : MonoBehaviour, ISelectHandler, IPointerEnterHandler
{
    public AudioClip clickSound;
    public AudioClip hoverSound;

    public void OnSelect(BaseEventData eventData)
    {
        if (clickSound != null)
        {
            FindFirstObjectByType<UISoundMaster>().audioSource.PlayOneShot(clickSound);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverSound != null)
        {
            FindFirstObjectByType<UISoundMaster>().audioSource.PlayOneShot(hoverSound);
        }
    }
}
