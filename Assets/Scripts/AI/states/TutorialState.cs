using UnityEngine;
using static Utils;

public class TutorialState : MonsterState
{
    private const float startDelay = 5f;
    private bool waiting = true;
    private bool initialized = false;
    private Vector2 lastPos;
    public TutorialState(MonsterStateMachine stateMachine, AIController controller) : base(stateMachine, controller) { }

    private void InitiateFirstEncounter()
    {
        float theta = camera.eulerAngles.y * Mathf.Deg2Rad + Mathf.PI;
        const float dist = 4.5f;
        Vector2 spawnLoc = new Vector2(camera.position.x + dist * Mathf.Sin(theta), camera.position.z + dist * Mathf.Cos(theta));
        controller.MoveToRock(spawnLoc);
        controller.ChangeMorph();
        controller.ToggleMorph(true);
    }

    public override void OnStateEnter()
    {
        if (GameSettings.includeTutorial)
        {
            Tutorial.PlayTutorialDelay(stateMachine, startDelay);
            waiting = true;
            RunDelay(stateMachine, () => waiting = false, startDelay);
            initialized = false;
        }
    }

    public override bool OnStateUpdate()
    {
        if (Tutorial.inFirstEncounter)
        {
            // initialize
            if (!initialized)
            {
                initialized = true;
                InitiateFirstEncounter();
                Object.FindFirstObjectByType<ProximityCue>().IsInRange(true);
            }

            // check look
            if (Tutorial.waitingTurn || Tutorial.waitingLookAt)
            {
                Vector3 diff3D = (controller.GetMorphPosition() - camera.position).normalized;
                float dot = Vector3.Dot(camera.forward, diff3D);
                if (Tutorial.waitingTurn && dot > 0.9) {
                    Tutorial.TurnAround();
                } else if (Tutorial.waitingLookAt && dot > 0.986f)
                {
                    lastPos = ToVector2(camera.position);
                    Tutorial.LookAt();
                }
            }

            // check backoff
            if (Tutorial.waitingBackOff)
            {
                float dist = (ToVector2(camera.position) - lastPos).magnitude;
                if (dist > 4f)
                {
                    Tutorial.BackedOff();
                    // Go to run state
                    return false;
                }
            }
        }
        return GameSettings.includeTutorial && (waiting || Tutorial.inProgress);
    }

    public override void OnStateExit()
    {
        waiting = true;
    }
}