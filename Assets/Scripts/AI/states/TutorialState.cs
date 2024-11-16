using UnityEngine;

public class TutorialState : MonsterState
{
    private Tutorial tut;
    private const float startDelay = 5f;
    private bool waiting = true;
    public TutorialState(MonsterStateMachine stateMachine, AIController controller) : base(stateMachine, controller) { }

    public override void OnStateEnter()
    {
        if (GameSettings.includeTutorial)
        {
            tut = Object.FindFirstObjectByType<Tutorial>();
            tut.PlayTutorialDelay(startDelay);
            waiting = true;
            Utils.RunDelay(stateMachine, () => waiting = false, startDelay);
        }
    }

    public override bool OnStateUpdate()
    {
        return GameSettings.includeTutorial && (waiting || tut.inProgress);
    }

    public override void OnStateExit()
    {
        waiting = true;
    }
}