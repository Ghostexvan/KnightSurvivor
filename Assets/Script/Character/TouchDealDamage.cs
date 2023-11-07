using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class TouchDealDamage : MonoBehaviour
{
    public CharacterData characterData;
    // Them cooldown
    public bool isCooldown;
    // Thoi gian cooldown
    public float cooldownTime;

    // Is used to check whether the enemy can attack again
    private float attackTimer;

    private void Update()
    {
        //attackTimer += Time.deltaTime;
    }

    public void OnCollisionEnter(Collision collision)
    {
        // Dang cooldown thi khong gay damage
        if (isCooldown)
            return;

        if (collision.gameObject.TryGetComponent(out Damageable damageObject) && collision.gameObject.tag == "Player"){
            // Check if the enemy can attack again
            damageObject.DealDamage(-characterData.attackDamage.Value);
            //if (attackTimer >= characterData.attackCooldown.Value)
            //{
            //    attackTimer = 0;
                
            //}
        }
    }

    // Might need to add condition for Bullet GameObj Tag?
    public void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out Damageable damageObject) && collision.gameObject.tag == "Player")
        {
            // Check if the enemy can attack again
            attackTimer += Time.deltaTime;        // If CollisionStay, will this increment?
            if (attackTimer >= characterData.attackCooldown.Value)
            {
                attackTimer = 0;
                damageObject.DealDamage(-characterData.attackDamage.Value);
            }

            /// The snippet above allowed the enemy to deal damage twice (From Enter and Stay) THEN deals
            /// damage at intervals
            /// So instead of that, I tried using an Enumerator (freaking thing really dealt IAI DAMAGE GOOD LORD)
            //StartCoroutine(TimedDamageDeal(damageObject));
        }
    }

    public void OnCollisionExit(Collision collision)
    {
        // Collision is still the one you'd just interacted btw (Meaning the Player collider)
        // Resets the attackTimer to 0, hence ending the OnCollisionStay damage dealing
        if (collision.gameObject.TryGetComponent(out Damageable damageObject) && collision.gameObject.tag == "Player")
        {
            attackTimer = 0;
            //StopCoroutine(TimedDamageDeal(damageObject));
        }
    }


    IEnumerator TimedDamageDeal(Damageable dmgObj)
    {
        yield return new WaitForSeconds(characterData.attackCooldown.Value);

        dmgObj.DealDamage(-characterData.attackDamage.Value);
    }

    public void SetData(CharacterData data){
        characterData = data;
    }
}
