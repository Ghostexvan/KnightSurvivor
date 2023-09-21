using System.Collections;
using UnityEditor;
using UnityEngine;

public class ArrowControl : MonoBehaviour
{
    public float attackDamage,
                 moveSpeed;
    private bool isCrit;

    private void Awake() {
        StartCoroutine(SelfDestroy());
    }

    private void FixedUpdate() {
        transform.Translate(-gameObject.transform.forward * moveSpeed * Time.deltaTime, Space.Self);
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
            Destroy(gameObject);
        }
    }

    IEnumerator SelfDestroy(){
        yield return new WaitForSeconds(10);
        Destroy(gameObject);
    }

    private void OnBecameInvisible() {
        Destroy(gameObject);
    }
}
