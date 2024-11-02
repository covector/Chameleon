using System;
using UnityEngine;

public abstract class MonsterState
{
    protected MonsterStateMachine stateMachine;
    protected Transform monster;
    protected Transform camera;
    protected AIController controller;
    public MonsterState(MonsterStateMachine stateMachine, AIController controller)
    {
        this.stateMachine = stateMachine;
        this.monster = stateMachine.gameObject.transform;
        this.camera = Camera.main.transform;
        this.controller = controller;
    }
    public abstract void OnStateEnter();
    public abstract bool OnStateUpdate();   // return false to exit
    public abstract void OnStateExit();
}
