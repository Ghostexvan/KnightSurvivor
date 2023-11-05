using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.Events;

public class RunState : IState
{
    public void OnEnter(StateController sc){
        sc.onRunStart.Invoke();
        sc.ani.Play("Move");
        Debug.Log("Move");
    }

    public void UpdateState(StateController sc){
        // Change state condition check
        if (sc.health.health <= 0)
            sc.ChangeState(sc.deathState);
            
        if (!sc.playerData.isOnGround){
            if (sc.playerData.isJump)
                sc.ChangeState(sc.jumpState);
            else
                sc.ChangeState(sc.fallState);
        }

        // if (sc.playerData.isAttack)
        //     sc.ChangeState(sc.attackState);

        // In state animation controller
        if (Time.realtimeSinceStartup >= sc.timeBeforeOut)
            sc.ani.SetFloat("Attack", 0.0f);

        if (sc.playerData.isMove)
            sc.ani.SetFloat("Speed", 1.0f);
        else
            sc.ani.SetFloat("Speed", 0.0f);

        ChangeMoveDirection(sc.playerData.moveDirection, sc.ani);
    }

    void ChangeMoveDirection(Direction direction, Animator ani){
        if ((float) direction >= ani.GetFloat("Direction") - 0.01f && (float) direction <= ani.GetFloat("Direction") + 0.01f)
            return;

        if ((float) direction > ani.GetFloat("Direction"))
            ani.SetFloat("Direction", ani.GetFloat("Direction") + 0.1f);
        else if ((float) direction < ani.GetFloat("Direction"))
            ani.SetFloat("Direction", ani.GetFloat("Direction") - 0.1f);
    }

    public void OnHurt(StateController sc){

    }

    public void OnExit(StateController sc){
        sc.ani.SetFloat("Direction", (int)Direction.Forward);
    }
}
