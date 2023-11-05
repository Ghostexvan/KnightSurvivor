using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public Rigidbody rigid;
    public float bulletSpeed;
    public int bulletDamage;

    private void Awake() {
        rigid = GetComponent<Rigidbody>();
        rigid.AddForce(transform.forward * bulletSpeed, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Destroyable"){
            other.gameObject.SendMessage("AddDamage", bulletDamage);
        }

        Destroy(this.gameObject);
    }
}
