using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallState : IState
{
    public void OnEnter(StateController sc){
        sc.onFallStart.Invoke();
        sc.ani.Play("Fall");
        Debug.Log("Fall");
    }

    public void UpdateState(StateController sc){
        if (sc.playerData.isOnGround)
            sc.ChangeState(sc.runState);
    }

    public void OnHurt(StateController sc){

    }

    public void OnExit(StateController sc){

    }
}
