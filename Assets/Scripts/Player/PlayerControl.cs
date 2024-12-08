using System;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public Camera cam;
    GradientController gradient;
    public TerrainGeneration tgen;
    float vShakeCounter = 0;
    public float vShakeMagnitude = 0.04f;
    public float vShakeFrequency = 8.6f;
    RandomAudio randomAudio;
    private bool canPlay = false;
    const float threshold = 0.5f * Mathf.PI;
    public 

    void Start()
    {
        randomAudio = GetComponent<RandomAudio>();
        gradient = new GradientController();
    }

    void Update()
    {
        if (PauseGame.instance.isFrozen) return;

        // Rotation Control
        UpdateRotation();

        // Movement Control
        float sqrDist = UpdateLocation();

        // Collision Check
        if (!Tutorial.waitingBackOff)  // prevent softlocking in tutorial lol
        {
            Vector2 collisionCorrected = tgen.CheckCollision(Utils.ToVector2(transform.position), 0.7f);
            transform.position = Utils.ProjectToGround(collisionCorrected, 2);
        }

        // Walking Vertical Shake
        vShakeCounter = Mathf.Repeat(vShakeCounter + Time.deltaTime * vShakeFrequency, 2f * Mathf.PI);
        float offset = sqrDist * vShakeMagnitude * (Mathf.Sin(vShakeCounter) - 0.5f);

        if (vShakeCounter < threshold) { canPlay = true; }
        if (canPlay && vShakeCounter >= threshold && sqrDist > 0.1f)
        {
            randomAudio.PlayRandomSound(sqrDist / 3f);
            canPlay = false;
        }

        // Intersection Check
        if (sqrDist > 0.05f) {
            tgen.CheckIntersection(Utils.ToVector2(transform.position), sqrDist);
        }

        cam.transform.position = transform.position + new Vector3(0f, 2f + offset, 0f);
    }

    private void UpdateRotation()
    {
        if (Tutorial.inFirstEncounter && Tutorial.lockTurn) { return; }
        float sensitivity = 4.0f * PlayerOptions.instance.Sensitivity;
        float rotateHorizontal = Input.GetAxis("Mouse X");
        float rotateVertical = Input.GetAxis("Mouse Y");
        cam.transform.eulerAngles = new Vector3(
            Mathf.Clamp(cam.transform.eulerAngles.x - rotateVertical * sensitivity,
                    cam.transform.eulerAngles.x < 180f ? -90f : 270f,
                    cam.transform.eulerAngles.x < 180f ? 90f : 400f),
            cam.transform.eulerAngles.y + rotateHorizontal * sensitivity,
            0
        );
    }

    private float UpdateLocation()
    {
        float vertical = Input.GetAxis("Vertical");
        if (Tutorial.waitingBackOff) { vertical = Mathf.Clamp(vertical, -1f, 0f); }
        else if (Tutorial.inFirstEncounter) { vertical = 0f; }
        vertical = vertical > 0 ? vertical : vertical * 0.4f;
        float horizontal = Input.GetAxis("Horizontal") * 0.4f;
        if (Tutorial.inFirstEncounter) { horizontal = 0f; }
        float length = Mathf.Sqrt(vertical * vertical + horizontal * horizontal);
        float verticalFac = vertical > 0 ? 1 / length : 0.4f / length;
        if (verticalFac < 1)
        {
            vertical *= verticalFac;
            horizontal *= verticalFac;
        }
        float sin = 3f * Mathf.Sin(cam.transform.eulerAngles.y * Mathf.PI / 180f);
        float cos = 3f * Mathf.Cos(cam.transform.eulerAngles.y * Mathf.PI / 180f);
        float xDel = vertical * sin + horizontal * cos;
        float zDel = vertical * cos - horizontal * sin;
        transform.position = gradient.GetAdjustedPosition(transform, xDel * Time.deltaTime, zDel * Time.deltaTime, levels: 2);
        float sqrDist = Mathf.Sqrt(xDel * xDel + zDel * zDel);
        return sqrDist;
    }
}
