using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpItem : MonoBehaviour
{
    public float rotateSpeed;
    public int expAmount;

    private void Awake() {
        Debug.Log("Item position: " + gameObject.transform.position + ", local: " + gameObject.transform.localPosition);
    }

    private void FixedUpdate() {
        if (IsOnGround()){
            gameObject.GetComponent<Rigidbody>().isKinematic = true;
        }
        else
            gameObject.GetComponent<Rigidbody>().isKinematic = false;
    }
    
    private void LateUpdate() {
        Rotate();
    }

    private bool IsOnGround()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1f);
    }

    private void Rotate(){
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player"){
            other.gameObject.GetComponent<CharacterLevel>().currentEXP += expAmount;
            Destroy(gameObject);
        }
    }
}
