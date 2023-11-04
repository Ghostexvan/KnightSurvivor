using System.Collections;
using UnityEngine;

public class TouchDealDamage : MonoBehaviour
{
    public CharacterData characterData;
    // Them cooldown
    public bool isCooldown;
    // Thoi gian cooldown
    public float cooldownTime;

    public void OnCollisionEnter(Collision collision)
    {
        // Dang cooldown thi khong gay damage
        if (isCooldown)
            return;

        if (collision.gameObject.TryGetComponent(out Damageable damageObject) && collision.gameObject.tag == "Player"){
            damageObject.DealDamage(-characterData.attackDamage.Value);

            // Gay damage xong thi cooldown
            StartCoroutine(Cooldown());
        }
    }

    IEnumerator Cooldown(){
        isCooldown = true;
        yield return new WaitForSeconds(cooldownTime);
        isCooldown = false;
    }

    public void SetData(CharacterData data){
        characterData = data;
    }
}
