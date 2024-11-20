using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Utils;

public class ApproachState : MonsterState
{
    private float lastDist = float.PositiveInfinity;
    private const float minHideDist = 15f;
    private bool isHiding = false;
    private const float speed = 3.4f;
    private bool isAFK;
    private const float afkChance = 0.05f;
    private const float afkSeconds = 2f;
    private Updater updater;
    private Vector2 nearestRock = Vector2.zero;
    private const float rockDirectFac = 5f;
    private ProximityCue proximityCue;

    public ApproachState(MonsterStateMachine stateMachine, AIController controller) : base(stateMachine, controller) {
        updater = new Updater(3f, UpdateNearestRock);
    }

    private Vector2 GetSpawnLocation()
    {
        float theta = Random.Range(0f, 2f * Mathf.PI);
        return new Vector2(camera.position.x + 30f * Mathf.Sin(theta), camera.position.z + 30f * Mathf.Cos(theta));
    }

    private void InitiateAFK(float seconds)
    {
        if (isAFK) { return; }
        isAFK = true;
        RunDelay(() => { isAFK = false; }, seconds);
    }

    private void OnSwitchHideState()
    {
        // the moment of switch
        if (!isHiding && Random.Range(0f, 1f) < afkChance)
        {
            Debug.Log("afk");
            InitiateAFK(afkSeconds);
        }
    }

    private void UpdateNearestRock()
    {
        float dist = float.PositiveInfinity;
        Vector2 monsterPos = ToVector2(monster.position);
        List<ProceduralAsset> rocks = TerrainGeneration.instance.GetNearAsset(monsterPos, (pa) => pa.ID() == AssetID.ROCK);
        foreach (ProceduralAsset rock in rocks)
        {
            Vector2 rockPos = ToVector2(rock.transform.position);
            float newDist = (rockPos - monsterPos).sqrMagnitude;
            if (newDist < dist)
            {
                dist = newDist;
                nearestRock = rockPos;
            }
        }
    }

    public override void OnStateEnter()
    {
        controller.MoveToRock(GetSpawnLocation());
        controller.ChangeMorph();
        lastDist = float.PositiveInfinity;
        controller.ToggleMorph(true);
        proximityCue = Object.FindFirstObjectByType<ProximityCue>();
    }

    public override bool OnStateUpdate()
    {
        Vector2 diff = controller.GetDiff(normalized: false);
        float dist = diff.magnitude;
        diff = diff / dist;

        // Moving
        if (!isHiding && !isAFK)
        {
            Vector2 playerGoal = diff;
            Vector2 rockGoal = (nearestRock - ToVector2(monster.position)).normalized;
            Vector2 finalGoal = Vector2.Dot(playerGoal, rockGoal) < 0 || dist > minHideDist ?
                playerGoal :
                (playerGoal + rockGoal * rockDirectFac).normalized
            ;
            Vector2 velocity = finalGoal * speed * Time.deltaTime;
            controller.MoveRock(velocity);
        }

        // Check distance
        if (dist > lastDist + 5f && dist < minHideDist)  // Backed off
        {
            return false;
        }
        if (dist < lastDist - 0.5f)  // Update lastDist
        {
            lastDist = Mathf.Max(dist, 2f);
        }

        // Check look direction
        //Vector3 diff3D = (controller.GetMorphPosition() - camera.position).normalized;
        float dot = Vector2.Dot(ToVector2(camera.forward), -diff);
        bool facing = dot > 0.01f;
        bool lastIsHiding = isHiding;
        isHiding = facing && dist < minHideDist;
        if (lastIsHiding != isHiding) { OnSwitchHideState(); }
        //controller.ToggleMorph(isHiding);

        proximityCue.IsInRange(dist < minHideDist + 2f);

        if (dist < minHideDist) { updater.Update(); }

        return true;
    }

    public override void OnStateExit()
    {
        controller.GetComponent<RandomAudio>().PlayRandomSound();
    }
}
