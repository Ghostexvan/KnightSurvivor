using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    public bool isActive;
    public UnityEvent<float> onDamaged;
    public CharacterData characterData;
    public GameObject damageText;
    public bool isCrit;

    public void DealDamage(float damageAmount){
        if (!isActive)
            return;
   
        onDamaged.Invoke(System.Math.Min(damageAmount + characterData.defense.Value, 0));
        //Debug.Log(damageAmount + characterData.defense.GetFinalValue());
        GameObject damageTextInstance = Instantiate(damageText, 
                                                    new Vector3(transform.position.x + Random.Range(-1f, 1f), 
                                                                transform.position.y + Random.Range(-1f, 1f), 
                                                                transform.position.z + Random.Range(-1f, -0.1f) - GetComponent<Collider>().bounds.size.z/2), 
                                                    Quaternion.identity);
        damageTextInstance.SendMessage("SetValue", System.Math.Max(-damageAmount - characterData.defense.Value, 0));
        if (isCrit)
            damageTextInstance.SendMessage("SetCrit");
        // onDamaged.Invoke(damageAmount);
    }

    public void SetCrit(bool isCrit){
        this.isCrit = isCrit;
    }

    public void Active(){
        isActive = true;
    }

    public void Deactive(){
        isActive = false;
    }
    
    public void SetData(CharacterData data){
        characterData = data;
    }
}
