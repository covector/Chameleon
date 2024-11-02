using System.Collections;
using UnityEngine;

using static Utils;

public class RunState : MonsterState
{
    private const float speed = 7f;
    private Vector2 goal { get; set; }
    private bool isRunning = false;

    public RunState(MonsterStateMachine stateMachine, AIController controller) : base(stateMachine, controller) { }

    public override void OnStateEnter()
    {
        stateMachine.StartCoroutine(Morph(0.2f));
        isRunning = false;
    }

    IEnumerator Morph(float time)
    {
        // Morph from rock
        float goalScale = 0.5f / controller.currentMorph.GetComponent<ProceduralAsset>().MaxDim();
        for (float t = 0f; t < time; t += Time.deltaTime)
        {
            controller.currentMorph.transform.localScale = Vector3.one * Mathf.Lerp(1f, goalScale, t / time);
            yield return null;
        }

        // Start running
        Vector2 diff = controller.GetDiff();
        goal = ToVector2(camera.position) - diff * 100f;
        controller.ToggleMorph(false);
        controller.PlayMonsterRunAnimation(() => isRunning = true);
    }

    public override bool OnStateUpdate()
    {
        if (isRunning)
        {
            Vector2 velocity = (goal - ToVector2(monster.position)).normalized * speed * Time.deltaTime;
            controller.Move(velocity);

            if (controller.GetDiff(normalized: false).sqrMagnitude > 2500)
            {
                return false;
            }
        }
        return true;
    }

    public override void OnStateExit()
    {
        controller.ToggleMorph(false);
        controller.ToggleMonster(false);
    }
}
