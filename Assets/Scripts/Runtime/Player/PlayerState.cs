using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerState
{
    protected readonly PlayerStateMachine FiniteStateMachine;

    public PlayerState(PlayerStateMachine finiteStateMachine)
    {
        FiniteStateMachine = finiteStateMachine;
    }

    //Initialize
    public virtual void OnEnter()
    {
        
    }

    //Reset Parameters
    public virtual void OnExit()
    {

    }

    //Executed on MonoBehaviour Update
    public virtual void OnUpdate()
    {

    }

    //Executed on MonoBehaviour FixedUpdate
    public virtual void OnFixedUpdate()
    {

    }
}
