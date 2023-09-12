using System.Collections;
using UnityEngine;

public class Attackable : MonoBehaviour
{
    public GameObject itemHand;
    public CharacterData characterData;
    public float attackDamage,
                 critChance,
                 critDamage,
                 coolDownTime;
    public bool isActive,
                isCooldown;

    private void Awake() {
        isCooldown = false;
    }

    private void Start() {
        gameObject.GetComponent<Animator>().SetInteger("AttackCount", (int)characterData.attackCount.Value);
    }

    private void FixedUpdate() {
        Attack();
        if (gameObject.GetComponent<Animator>().GetInteger("AttackCount") == -1 && !isCooldown){
            StartCoroutine(CooldownAttack());
        }
    }

    void GetAttack(){
        attackDamage = characterData.attackDamage.Value;
        critChance = characterData.critChance.Value;
        critDamage = characterData.critDamage.Value;
    }

    public void Attack(){
        if (!isActive || isCooldown){
            return;
        }

        GetAttack();
        
        float totalDamage;
        if (Random.Range(0, 100) <= critChance)
            totalDamage = -attackDamage * (1 + critDamage/100);
        else
            totalDamage = -attackDamage;

        Debug.Log(totalDamage);
        
        itemHand.BroadcastMessage("SetDamage", totalDamage);
        itemHand.BroadcastMessage("SetIsCrit", -totalDamage > characterData.attackDamage.Value);
    }

    IEnumerator CooldownAttack(){
        isCooldown = true;
        yield return new WaitForSeconds(characterData.attackCooldown.Value);
        gameObject.GetComponent<Animator>().SetInteger("AttackCount", (int)characterData.attackCount.Value);
        isCooldown = false;
    }

    public void ActiveTrigger(){
        itemHand.BroadcastMessage("ActiveTrigger");
    }

    public void DeactiveTrigger(){
        itemHand.BroadcastMessage("DeactiveTrigger");
    }

    public void SetData(CharacterData data){
        characterData = data;
    }

    public void Active(){
        isActive = true;
    }

    public void Deactive(){
        isActive = false;
    }
}
