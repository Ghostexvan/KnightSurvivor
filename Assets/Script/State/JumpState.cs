using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState : IState
{
    public void OnEnter(StateController sc){
        sc.onJumpStart.Invoke();
        sc.ani.Play("Jump");
        Debug.Log("Jump");
    }

    public void UpdateState(StateController sc){
        if (!sc.playerData.isOnGround && !sc.playerData.isJump)
            sc.ChangeState(sc.fallState);

        if (sc.playerData.isOnGround)
            sc.ChangeState(sc.runState);
    }

    public void OnHurt(StateController sc){

    }

    public void OnExit(StateController sc){

    }
}
