using System.Collections;
using UnityEngine;

public class MeleeWeaponControl : MonoBehaviour
{
    public float attackDamage;
    private bool isCrit;

    private void Awake() {
        DeactiveTrigger();
    }

    public IEnumerator Attack(){
        ActiveTrigger();
        yield return new WaitForSeconds(0.1f);
        DeactiveTrigger();
    }

    public void ActiveTrigger(){
        gameObject.GetComponent<Collider>().enabled = true;
    }

    public void DeactiveTrigger(){
        gameObject.GetComponent<Collider>().enabled = false;
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
