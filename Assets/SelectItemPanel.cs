using System.Collections.Generic;
using UnityEngine;

public class SelectItemPanel : MonoBehaviour
{
    public GameObject character,
                      panel,
                      prefab;
    public bool isSet;

    public List<ItemInstance> itemList = new List<ItemInstance>();

    private void Awake() {
        panel = gameObject.transform.GetChild(1).gameObject;
    }

    private void Start() {
        gameObject.SetActive(false);
    }

    private void Update() {
        if (isSet){
            for (int index = 0; index < itemList.Count; index++){
                GameObject itemPanel = Instantiate(prefab, panel.transform);
                itemPanel.GetComponent<WeaponSelect>().itemInstance = itemList[index];
                itemPanel.GetComponent<WeaponSelect>().character = character;
                itemPanel.GetComponent<WeaponSelect>().isSet = true;
            }

            isSet = false;
            Time.timeScale = 0f;
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
