// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class AttackState : IState
// {
//     public float activeTime;
//     private float[] attackType = {0f, 0.5f, 1f};

//     public void OnEnter(StateController sc){
//         sc.onAttackStart.Invoke();

//         sc.ani.SetFloat("AttackType", attackType[Random.Range(0, 2)]);
//         sc.ani.SetFloat("Attack", 1.0f);
//         sc.ani.Play("Attack1");
//         sc.ani.SetInteger("AttackCount", GameObject.FindGameObjectWithTag("Player").GetComponent<Attackable>().attackCount);
        
//         Debug.Log("Attack");
//     }

//     public void UpdateState(StateController sc){
//         if (!sc.playerData.isAttack)
//             sc.ChangeState(sc.runState);

//         if (!sc.ani.GetCurrentAnimatorStateInfo(0).IsName("Attack")){
//             sc.playerData.isAttack = false;
//         }
//     }

//     public void OnHurt(StateController sc){

//     }

//     public void OnExit(StateController sc){

//     }
// }
