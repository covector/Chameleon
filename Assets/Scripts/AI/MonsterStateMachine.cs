using System.Collections.Generic;
using UnityEngine;

public class MonsterStateMachine : MonoBehaviour
{
    public enum State
    {
        Tutorial, Idle, Approach, Run, Jumpscare
    }

    public Dictionary<State, MonsterState> states { get; private set; }
    public State current { get; private set; }
    private AIController controller;
    public bool intercept { get; set; }

    private void Start()
    {
        controller = GetComponent<AIController>();
        states = new Dictionary<State, MonsterState>();
        states[State.Tutorial] = new TutorialState(this, controller);
        states[State.Idle] = new IdleState(this, controller);
        states[State.Approach] = new ApproachState(this, controller);
        states[State.Run] = new RunState(this, controller);
        states[State.Jumpscare] = new JumpscareState(this, controller);
        InitState();
        states[current].OnStateEnter();
        intercept = true;
    }

    private void Update()
    {
        if (intercept) { InterceptState(); }

        if (!states[current].OnStateUpdate())
        {
            states[current].OnStateExit();
            ToNewState();
            states[current].OnStateEnter();
        }
    }

    private void InitState()
    {
        current = State.Tutorial;
        ((IdleState)states[State.Idle]).waitTime = 5f;
    }

    private void ToNewState()
    {
        switch (current)
        {
            case State.Tutorial:
                current = State.Idle;
                break;
            case State.Idle:
                current = State.Approach;
                current = State.Jumpscare;
                intercept = false;
                break;
            case State.Approach:
                current = State.Run;
                break;
            case State.Run:
                current = State.Idle;
                ((IdleState)states[State.Idle]).waitTime = 15f;
                break;
        }
        Debug.Log(current);
    }

    private void InterceptState()
    {
        if (current != State.Idle && current != State.Tutorial && controller.IsLost())
        {
            states[current].OnStateExit();
            current = State.Jumpscare;
            intercept = false;
            states[current].OnStateEnter();
        }
    }
}
