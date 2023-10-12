using UnityEngine;

public class RangedWeaponControl : MonoBehaviour
{
    public GameObject arrow;
    public Transform position;
    public float attackDamage;
    private bool isCrit;

    private void Awake() {
        position = gameObject.transform.GetChild(0);
    }

    public void Attack(){
        GameObject arrowInstance = Instantiate(arrow, position.position, Quaternion.identity, GameObject.Find("Projectiles").transform);
        arrowInstance.transform.localEulerAngles = new Vector3(90f, GameObject.FindGameObjectWithTag("Player").transform.eulerAngles.y, 0f);
        arrowInstance.SendMessage("SetDamage", attackDamage);
        arrowInstance.SendMessage("SetIsCrit", isCrit);
    }

    public void SetDamage(float damage){
        attackDamage = damage;
    }

    public void SetIsCrit(bool isCrit){
        this.isCrit = isCrit;
    }
}
