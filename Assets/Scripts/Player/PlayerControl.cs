using UnityEngine;

//[RequireComponent(typeof(Rigidbody))]
public class PlayerControl : MonoBehaviour
{
    public Camera cam;
    public TerrainGeneration tgen;
    Rigidbody rb;
    float lastY = 0f;

    void Start()
    {
        //rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float sensitivity = 1000.0f;
        float rotateHorizontal = Input.GetAxis("Mouse X");
        float rotateVertical = Input.GetAxis("Mouse Y");
        cam.transform.eulerAngles = new Vector3(
            Mathf.Clamp(cam.transform.eulerAngles.x - rotateVertical * sensitivity * Mathf.Min(1f/60f, Time.deltaTime), cam.transform.eulerAngles.x < 180f ? -90f : 270f,
            cam.transform.eulerAngles.x < 180f ? 90f : 400f), cam.transform.eulerAngles.y + rotateHorizontal * sensitivity * Mathf.Min(1f / 60f, Time.deltaTime),
            0);

        //rb.AddForce(new Vector3(Input.GetAxis("Vertical") * Mathf.Sin(cam.transform.eulerAngles.y * Mathf.PI / 180f), 0f, Input.GetAxis("Vertical") * Mathf.Cos(cam.transform.eulerAngles.y * Mathf.PI / 180f)));
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
        float xDel = 3f * vertical * Mathf.Sin(cam.transform.eulerAngles.y * Mathf.PI / 180f) * Time.deltaTime;
        float zDel = 3f * vertical * Mathf.Cos(cam.transform.eulerAngles.y * Mathf.PI / 180f) * Time.deltaTime;
        xDel += 3f * horizontal * Mathf.Cos(cam.transform.eulerAngles.y * Mathf.PI / 180f) * Time.deltaTime;
        zDel -= 3f * horizontal * Mathf.Sin(cam.transform.eulerAngles.y * Mathf.PI / 180f) * Time.deltaTime;
            //transform.position = new Vector3(
            //    transform.position.x + xDel,
            //    tgen.GetGroudLevel(transform.position.x + xDel, transform.position.z + zDel, 2) + 1f,
            //    transform.position.z + zDel
            //);
        float y = tgen.GetGroudLevel(transform.position.x + xDel, transform.position.z + zDel, 1) + 1f;
        float gradient = Mathf.Max((y - lastY) / Time.deltaTime + 3f, 2.5f) / 3f;
        transform.position = new Vector3(
            transform.position.x + xDel / gradient,
            tgen.GetGroudLevel(transform.position.x + xDel / gradient, transform.position.z + zDel / gradient, 2) + 1f,
            transform.position.z + zDel / gradient
        );
        lastY = y;

        cam.transform.position = transform.position + new Vector3(0f, 1f, 0f);
    }
}
