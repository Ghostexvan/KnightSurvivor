using UnityEngine;

public class TouchDealDamage : MonoBehaviour
{
    public CharacterData characterData;

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out Damageable damageObject) && collision.gameObject.tag == "Player"){
            damageObject.DealDamage(-characterData.attackDamage.Value);
        }
    }

    public void SetData(CharacterData data){
        characterData = data;
    }
}
