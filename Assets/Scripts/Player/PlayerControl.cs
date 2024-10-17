using System;
using UnityEngine;

//[RequireComponent(typeof(Rigidbody))]
public class PlayerControl : MonoBehaviour
{
    public Camera cam;
    GradientController gradient;

    void Start()
    {
        gradient = new GradientController();
    }

    void Update()
    {
        // Rotation Control
        float sensitivity = 1000.0f;
        float rotateHorizontal = Input.GetAxis("Mouse X");
        float rotateVertical = Input.GetAxis("Mouse Y");
        float dt = Mathf.Min(1f / 60f, Time.deltaTime);
        cam.transform.eulerAngles = new Vector3(
            Mathf.Clamp(cam.transform.eulerAngles.x - rotateVertical * sensitivity * dt,
                    cam.transform.eulerAngles.x < 180f ? -90f : 270f,
                    cam.transform.eulerAngles.x < 180f ? 90f : 400f),
            cam.transform.eulerAngles.y + rotateHorizontal * sensitivity * dt,
            0
        );

        // Movement Control
        float vertical = Input.GetAxis("Vertical");
        vertical = vertical > 0 ? vertical : vertical * 0.4f;
        float horizontal = Input.GetAxis("Horizontal") * 0.4f;
        float length = Mathf.Sqrt(vertical * vertical + horizontal * horizontal);
        float verticalFac = vertical > 0 ? 1 / length : 0.4f / length;
        if (verticalFac < 1)
        {
            vertical *= verticalFac;
            horizontal *= verticalFac;
        }
        float sin = 3f * Mathf.Sin(cam.transform.eulerAngles.y * Mathf.PI / 180f) * Time.deltaTime;
        float cos = 3f * Mathf.Cos(cam.transform.eulerAngles.y * Mathf.PI / 180f) * Time.deltaTime;
        float xDel = vertical * sin + horizontal * cos;
        float zDel = vertical * cos - horizontal * sin;
        transform.position = gradient.GetAdjustedPosition(transform, xDel, zDel, levels:2);
        cam.transform.position = transform.position + new Vector3(0f, 2f, 0f);
    }
}
