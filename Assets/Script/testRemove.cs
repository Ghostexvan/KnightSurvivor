using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testRemove : MonoBehaviour
{
    public GameObject player;

    private void Awake() {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void OnClick(){
        player.SendMessage("RemoveWeapon");
    }
}
