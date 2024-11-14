using System.Collections.Generic;
using UnityEngine;

public class MonsterStateMachine : MonoBehaviour
{
    public Dictionary<string, MonsterState> states { get; private set; }
    public string current { get; private set; }
    private AIController controller;
    public bool intercept { get; set; }

    private void Start()
    {
        controller = GetComponent<AIController>();
        states = new Dictionary<string, MonsterState>();
        states["Idle"] = new IdleState(this, controller);
        states["Approach"] = new ApproachState(this, controller);
        states["Run"] = new RunState(this, controller);
        states["Jumpscare"] = new JumpscareState(this, controller);
        InitState();
        states[current].OnStateEnter();
        intercept = true;
    }

    private void Update()
    {
        if (intercept) { InterceptState(); }

        if (current != null && !states[current].OnStateUpdate())
        {
            states[current].OnStateExit();
            ToNewState();
            states[current].OnStateEnter();
        }
    }

    private void InitState()
    {
        current = "Idle";
        ((IdleState)states["Idle"]).waitTime = 5f;
    }

    private void ToNewState()
    {
        switch (current)
        {
            case "Idle":
                current = "Approach";
                break;
            case "Approach":
                current = "Run";
                break;
            case "Run":
                current = "Idle";
                ((IdleState)states["Idle"]).waitTime = 15f;
                break;
        }
        Debug.Log(current);
    }

    private void InterceptState()
    {
        if (controller.IsLost())
        {
            states[current].OnStateExit();
            current = "Jumpscare";
            intercept = false;
            states[current].OnStateEnter();
        }
    }
}
