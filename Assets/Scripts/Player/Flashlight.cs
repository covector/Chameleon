using UnityEngine;

public class Flashlight : MonoBehaviour
{
    private Transform cam;
    void Start()
    {
        cam = Camera.main.transform;
    }

    void Update()
    {
        transform.SetPositionAndRotation(cam.position, Quaternion.Lerp(transform.rotation, cam.rotation, Mathf.Min(1f, 20f * Time.deltaTime)));
    }
}
