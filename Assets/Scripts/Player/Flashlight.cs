using UnityEngine;

public class Flashlight : MonoBehaviour
{
    private Transform camera;
    void Start()
    {
        camera = Camera.main.transform;
    }

    void Update()
    {
        transform.SetPositionAndRotation(camera.position, Quaternion.Lerp(transform.rotation, camera.rotation, Mathf.Min(1f, 20f * Time.deltaTime)));
    }
}
