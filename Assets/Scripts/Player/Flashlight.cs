using UnityEngine;

public class Flashlight : MonoBehaviour
{
    private Transform cam;
    private Light spotLight;
    void Start()
    {
        cam = Camera.main.transform;
        spotLight = GetComponent<Light>();
    }

    void Update()
    {
        transform.SetPositionAndRotation(cam.position, Quaternion.Lerp(transform.rotation, cam.rotation, Mathf.Min(1f, 20f * Time.deltaTime)));
    }

    public void ToggleFlashlight(bool flash)
    {
        spotLight.enabled = flash;
    }
}
