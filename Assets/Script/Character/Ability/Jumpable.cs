using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class Jumpable : MonoBehaviour
{
    public CharacterData characterData;
    public Rigidbody rigid;
    public float jumpHeight;
    public bool isActive;

    private void Awake() {
        rigid = GetComponent<Rigidbody>();
    }

    private void FixedUpdate() {
        if (characterData == null)
            return;

        if (characterData.isOnGround || rigid.velocity.y <= 0)
            characterData.isJump = false;
        else if (!characterData.isOnGround && rigid.velocity.y > 0)
            characterData.isJump = true;
    }
    
    public void Jump(){
        if (characterData == null)
            return;
            
        if (!characterData.isOnGround || !isActive)
            return;

        rigid.velocity = new Vector3(rigid.velocity.x,
                                     jumpHeight,
                                     rigid.velocity.z);

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
