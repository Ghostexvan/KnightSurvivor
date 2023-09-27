using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SelectItemPanel : MonoBehaviour
{
    public GameObject character,
                      panel,
                      prefab;
    public bool isSet;
    public int currentIndex;

    public List<ItemInstance> itemList = new List<ItemInstance>();
    public List<GameObject> panelList = new List<GameObject>();

    private void Awake() {
        panel = gameObject.transform.GetChild(1).gameObject;
    }

    private void Start() {
        gameObject.SetActive(false);
    }

    private void Update() {
        if (isSet){
            currentIndex = 0;
            for (int index = 0; index < itemList.Count; index++){
                GameObject itemPanel = Instantiate(prefab, panel.transform);
                itemPanel.GetComponent<WeaponSelect>().itemInstance = itemList[index];
                itemPanel.GetComponent<WeaponSelect>().character = character;
                itemPanel.GetComponent<WeaponSelect>().isSet = true;
                itemPanel.GetComponent<WeaponSelect>().index = index;
                panelList.Add(itemPanel);
            }

            isSet = false;

            // Stops game time / Pauses the game while UI is on screen - when UI isSet
            Time.timeScale = 0f;
        }

        HighLightIndex();
    }

    private void OnDisable() {
        panelList.Clear();
    }

    public void SetPlayer(GameObject player){
        character = player;
    }

    public void SetItemList(List<ItemInstance> list){
        itemList = list;
        isSet = true;
    }

    public void SetCurrentIndex(int index){
        currentIndex = index;
    }

    public void HighLightIndex(){
        foreach (GameObject panel in panelList){
            if (panel.GetComponent<WeaponSelect>().index == currentIndex){
                panel.GetComponent<WeaponSelect>().HighLight(true);
            }
            else
                panel.GetComponent<WeaponSelect>().HighLight(false);
        }
    }

    public void ChangeIndex(int step){
        Debug.Log("Step: "+ step);
        if (step < 0){
            if (currentIndex + step < 0){
                currentIndex = panelList.Count - 1;
            }
            else{
                currentIndex += step;
            }
        }
        else if (step > 0) {
            if (currentIndex + step > panelList.Count - 1)
                currentIndex = 0;
            else
                currentIndex += step;
        }
    }

    public void ReadValueForChangeIndex(InputAction.CallbackContext context){
        if (context.started){
            Vector2 value = context.ReadValue<Vector2>();
            Debug.Log("Value: " + value);
            ChangeIndex(- (int) value.y);
        }
    }

    public void ConfirmSelect(InputAction.CallbackContext context){
        if (context.started){
            Debug.Log("Select");
            panelList[currentIndex].GetComponent<WeaponSelect>().OnClick();
        }
    }

    public void UDPConfirmSelect(bool accept)
    {
        if (accept)
        {
            panelList[currentIndex].GetComponent<WeaponSelect>().OnClick();
        }
    }
}
