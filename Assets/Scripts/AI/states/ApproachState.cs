using UnityEngine;
using static Utils;

public class ApproachState : MonsterState
{
    private float lastDist = float.PositiveInfinity;
    private const float minHideDist = 15f;
    private bool isHiding = false;
    private const float speed = 3.2f;

    public ApproachState(MonsterStateMachine stateMachine, AIController controller) : base(stateMachine, controller) { }

    private Vector2 GetSpawnLocation()
    {
        float theta = Random.Range(0f, 2f * Mathf.PI);
        return new Vector2(camera.position.x + 30f * Mathf.Sin(theta), camera.position.z + 30f * Mathf.Cos(theta));
    }

    public override void OnStateEnter()
    {
        controller.MoveTo(GetSpawnLocation());
        controller.ChangeMorph();
        lastDist = float.PositiveInfinity;
    }

    public override bool OnStateUpdate()
    {
        // Moving
        if (!isHiding)
        {
            Vector2 velocity = controller.GetDiff() * speed * Time.deltaTime;
            controller.Move(velocity);
        }

        // Check distance
        Vector2 diff = ToVector2(monster.position - camera.position);
        float dist = diff.magnitude;
        if (dist > lastDist + 5f && dist < minHideDist)  // Backed off
        {
            return false;
        }
        if (dist < lastDist - 0.5f)  // Update lastDist
        {
            lastDist = dist;
        }

        // Check look direction
        float dot = Vector2.Dot(ToVector2(camera.forward), diff);
        bool facing = dot > 0f;
        isHiding = facing && dist < minHideDist;
        controller.ToggleMorph(isHiding);

        return true;
    }

    public override void OnStateExit()
    {
        controller.GetComponent<RandomAudio>().PlayRandomSound();
    }
}
