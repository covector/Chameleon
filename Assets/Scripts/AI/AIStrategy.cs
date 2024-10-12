using System.Collections;
using UnityEngine;

public class AIStrategy : MonoBehaviour
{
    private Transform cam;
    private AIController controller;
    private bool approaching = false;
    private float lastDist = float.PositiveInfinity;
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
        lastDist = float.PositiveInfinity;
    }

    void OnApproaching()
    {
        Vector2 diff = Utils.ToVector2(transform.position - cam.position);
        float dist = diff.magnitude;
        if (dist > lastDist + 10f)
        {
            EndApproaching();
            return;
        }
        if (dist < lastDist - 0.5f)
        {
            lastDist = dist;
        }
        float dot = Vector2.Dot(Utils.ToVector2(cam.forward), diff);
        bool facing = dot > 0f;
        if (facing)
        {
            controller.Hide();
        } else
        {
            controller.Unhide();
        }
    }

    void EndApproaching()
    {
        approaching = false;
        controller.RunAway();
        StartCoroutine(ScheduleApproach(15));
    }
}
