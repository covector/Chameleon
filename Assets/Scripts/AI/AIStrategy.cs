using System.Collections;
using UnityEngine;

public class AIStrategy : MonoBehaviour
{
    private Transform cam;
    private AIController controller;
    private bool approaching = false;
    void Start()
    {
        cam = Camera.main.transform;
        controller = GetComponent<AIController>();
        StartCoroutine(ScheduleApproach(5));
    }

    void Update()
    {
        if (approaching)
        {
            OnApproaching();
        }
    }

    IEnumerator ScheduleApproach(int delay)
    {
        yield return new WaitForSeconds(delay);
        StartApproaching();
    }

    void StartApproaching()
    {
        float randomAngle = Random.Range(0f, 2 * Mathf.PI);
        transform.position = new Vector3(cam.position.x + 30f * Mathf.Cos(randomAngle), 0f, cam.position.z + 30f * Mathf.Sin(randomAngle));
        controller.ApproachPlayer();
        approaching = true;
    }

    void OnApproaching()
    {
        float dot = Vector3.Dot(cam.forward, (transform.position - cam.position));
        bool facing = dot > 0f;
        if (facing)
        {
            controller.Hide();
        } else
        {
            controller.Unhide();
        }
    }
}
