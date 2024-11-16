using UnityEngine;
using UnityEngine.SceneManagement;

public class JumpscareState : MonsterState
{
    public JumpscareState(MonsterStateMachine stateMachine, AIController controller) : base(stateMachine, controller) { }

    public override void OnStateEnter()
    {
        controller.ToggleMorph(false);
        controller.ToggleMonster(false);
        Object.FindFirstObjectByType<DeathCoordinator>().StartDeathScene();
    }

    public override bool OnStateUpdate()
    {
        //Vector2 diff = controller.GetDiff(normalized: false);
        //monster.transform.eulerAngles = new Vector3(0, Mathf.Atan2(diff.x, diff.y) * Mathf.Rad2Deg + 180f, 0);
        controller.CorrectToMonster();
        return true;
    }

    public override void OnStateExit()
    {
    }
}
