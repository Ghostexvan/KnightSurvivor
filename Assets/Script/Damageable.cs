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
   
        //Debug.Log(damageAmount + characterData.defense.GetFinalValue());
        GameObject damageTextInstance = Instantiate(damageText, 
                                                    new Vector3(gameObject.transform.position.x + Random.Range(-1f, 1f), 
                                                                gameObject.transform.position.y + Random.Range(0f, 2f), 
                                                                gameObject.transform.position.z + Random.Range(-1f, -0.1f) - GetComponent<Collider>().bounds.size.z/2
                                                    ),
                                                    Quaternion.identity,
                                                    gameObject.transform);
        //damageTextInstance.transform.localPosition = transform.TransformPoint(gameObject.transform.position);
        damageTextInstance.SendMessage("SetValue", System.Math.Max(-damageAmount - characterData.defense.Value, 0));
        Debug.Log("Expect text position: " + gameObject.transform.position);
        if (isCrit)
            damageTextInstance.SendMessage("SetCrit");
        onDamaged.Invoke(System.Math.Min(damageAmount + characterData.defense.Value, 0));
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
