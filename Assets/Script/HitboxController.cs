using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxController : MonoBehaviour
{
    public Collider collider;
    public float attackDamage;
    private bool isCrit;

    private void Awake() {
        collider = GetComponent<Collider>();
    }

    public void ActiveTrigger(){
        collider.enabled = true;
    }

    public void DeactiveTrigger(){
        collider.enabled = false;
    }

    public void SetDamage(float damage){
        attackDamage = damage;
    }

    public void SetIsCrit(bool isCrit){
        this.isCrit = isCrit;
    }

    public void OnTriggerEnter(Collider other) {
        if (other.gameObject.TryGetComponent(out Damageable damageObject) && other.gameObject.tag != "Player"){
            damageObject.gameObject.BroadcastMessage("SetCrit", isCrit);
            damageObject.DealDamage(attackDamage);
        }
    }
}
