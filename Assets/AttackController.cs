using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    public void OnAttackAnimationEvent(){
        this.SendMessage("Active");
    }

    public void ExitAttackAnimationEvent(){
        this.SendMessage("Deactive");
    }
}
