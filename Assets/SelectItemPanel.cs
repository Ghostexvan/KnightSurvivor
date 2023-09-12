using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectItemPanel : MonoBehaviour
{
    public GameObject character,
                      panel1,
                      panel2,
                      panel3;
    public bool isSet;

    public List<ItemInstance> itemList = new List<ItemInstance>();

    private void Start() {
        gameObject.SetActive(false);
    }

    private void FixedUpdate() {
        if (isSet){
            if (itemList.Count >= 1){
                panel1.GetComponent<WeaponSelect>().itemInstance = itemList[0];
                panel1.GetComponent<WeaponSelect>().character = character;
            }
            panel1.GetComponent<WeaponSelect>().isSet = true;
            
            if (itemList.Count >= 2){
                panel2.GetComponent<WeaponSelect>().itemInstance = itemList[1];
                panel2.GetComponent<WeaponSelect>().character = character;
            }
            panel2.GetComponent<WeaponSelect>().isSet = true;

            if (itemList.Count >= 3){
                panel3.GetComponent<WeaponSelect>().itemInstance = itemList[2];
                panel3.GetComponent<WeaponSelect>().character = character;
            }
            panel3.GetComponent<WeaponSelect>().isSet = true;
            isSet = false;
        }
    }

    public void SetPlayer(GameObject player){
        character = player;
    }

    public void SetItemList(List<ItemInstance> list){
        itemList = list;
        isSet = true;
    }
}
