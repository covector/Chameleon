using static Utils;

public class IdleState : MonsterState
{
    private bool waiting = true;
    public float waitTime {  get; set; }
    public IdleState(MonsterStateMachine stateMachine, AIController controller) : base(stateMachine, controller) { }

    public override void OnStateEnter()
    {
        waiting = true;
        RunDelay(stateMachine, () => waiting = false, waitTime);
    }

    public override bool OnStateUpdate()
    {
        return waiting;
    }

    public override void OnStateExit()
    {
        waiting = true;
    }
}
