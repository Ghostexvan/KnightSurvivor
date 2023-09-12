using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class WeaponSelect : MonoBehaviour
{
    public ItemInstance itemInstance;
    public GameObject character;
    public bool isSet;
    public TMP_Text itemName, 
                    description,
                    stat;

    private void Awake() {
        itemName = gameObject.transform.GetChild(1).transform.GetChild(0).gameObject.GetComponent<TMP_Text>();
        description = gameObject.transform.GetChild(1).transform.GetChild(1).gameObject.GetComponent<TMP_Text>();
        stat = gameObject.transform.GetChild(1).transform.GetChild(2).gameObject.GetComponent<TMP_Text>();
    }

    private void FixedUpdate() {
        if (itemInstance == null && isSet){
            gameObject.SetActive(false);
            isSet = false;
        } else if (itemInstance != null){
            gameObject.transform.GetChild(0).gameObject.GetComponent<RawImage>().texture = itemInstance.itemType.icon.texture;
            itemName.text = itemInstance.itemType.itemName;
            description.text = itemInstance.itemType.description;

            stat.text = "";
            if (itemInstance.itemType.GetType().Name == "WeaponData" && character.GetComponent<Equipment>().weapon.itemType == itemInstance.itemType){
                for (int index = 0; index < itemInstance.itemType.itemStats.Count; index++){
                    if (itemInstance.itemType.itemStats[index].level == character.GetComponent<Equipment>().weapon.currentLevel + 1){
                        if (stat.text != "")
                            stat.text += "; ";
                        stat.text += itemInstance.itemType.itemStats[index].statAffect.ToString();
                        stat.text += ": ";

                        if (itemInstance.itemType.itemStats[index].statType == StatModType.Flat || itemInstance.itemType.itemStats[index].statType == StatModType.PercentAdd){
                            if (itemInstance.itemType.itemStats[index].value >= 0)
                                stat.text += "+";
                        }
                        else {
                            stat.text += "x";
                        }

                        stat.text += itemInstance.itemType.itemStats[index].value;

                        if (itemInstance.itemType.itemStats[index].statType == StatModType.PercentAdd || itemInstance.itemType.itemStats[index].statType == StatModType.PercentMult){
                            stat.text += "%";
                        }
                    }
                }
            }
            else if (itemInstance.itemType.GetType().Name == "WeaponData"){
                for (int index = 0; index < itemInstance.itemType.itemStats.Count; index++){
                    if (itemInstance.itemType.itemStats[index].level == 1){
                        if (stat.text != "")
                            stat.text += "; ";
                        stat.text += itemInstance.itemType.itemStats[index].statAffect.ToString();
                        stat.text += ": ";

                        if (itemInstance.itemType.itemStats[index].statType == StatModType.Flat || itemInstance.itemType.itemStats[index].statType == StatModType.PercentAdd){
                            if (itemInstance.itemType.itemStats[index].value >= 0)
                                stat.text += "+";
                        }
                        else {
                            stat.text += "x";
                        }

                        stat.text += itemInstance.itemType.itemStats[index].value;

                        if (itemInstance.itemType.itemStats[index].statType == StatModType.PercentAdd || itemInstance.itemType.itemStats[index].statType == StatModType.PercentMult){
                            stat.text += "%";
                        }
                    }
                }
            }

            for (int indexItem = 0; indexItem < character.GetComponent<Equipment>().item.Count; indexItem++){
                if (itemInstance.itemType.GetType().Name == "ItemData" && character.GetComponent<Equipment>().item[indexItem].itemType == itemInstance.itemType){
                    for (int index = 0; index < itemInstance.itemType.itemStats.Count; index++){
                        if (itemInstance.itemType.itemStats[index].level == character.GetComponent<Equipment>().item[indexItem].currentLevel + 1){
                            if (stat.text != "")
                                stat.text += "; ";
                            stat.text += itemInstance.itemType.itemStats[index].statAffect.ToString();
                            stat.text += ": ";

                            if (itemInstance.itemType.itemStats[index].statType == StatModType.Flat || itemInstance.itemType.itemStats[index].statType == StatModType.PercentAdd){
                                if (itemInstance.itemType.itemStats[index].value >= 0)
                                    stat.text += "+";
                            }
                            else {
                                stat.text += "x";
                            }

                            stat.text += itemInstance.itemType.itemStats[index].value;

                            if (itemInstance.itemType.itemStats[index].statType == StatModType.PercentAdd || itemInstance.itemType.itemStats[index].statType == StatModType.PercentMult){
                                stat.text += "%";
                            }
                        }
                    }
                    break;
                }
            }

            if (stat.text == "" && itemInstance.itemType.GetType().Name == "ItemData"){
                for (int index = 0; index < itemInstance.itemType.itemStats.Count; index++){
                    if (itemInstance.itemType.itemStats[index].level == 1){
                        if (stat.text != "")
                            stat.text += "; ";
                        stat.text += itemInstance.itemType.itemStats[index].statAffect.ToString();
                        stat.text += ": ";

                        if (itemInstance.itemType.itemStats[index].statType == StatModType.Flat || itemInstance.itemType.itemStats[index].statType == StatModType.PercentAdd){
                            if (itemInstance.itemType.itemStats[index].value >= 0)
                                stat.text += "+";
                        }
                        else {
                            stat.text += "x";
                        }

                        stat.text += itemInstance.itemType.itemStats[index].value;

                        if (itemInstance.itemType.itemStats[index].statType == StatModType.PercentAdd || itemInstance.itemType.itemStats[index].statType == StatModType.PercentMult){
                            stat.text += "%";
                        }
                    }
                }
            }
        }
            
    }

    private void OnEnable() {
        itemInstance = null;
        character = null;
    }

    public void OnClick(){
        character.GetComponent<Equipment>().Equip(itemInstance);
        gameObject.transform.parent.transform.parent.gameObject.SetActive(false);
    }

    public void SetCharacterData(GameObject character){
        this.character = character;
    }

    public void SetItemInstance(ItemInstance itemInstance){
        this.itemInstance = itemInstance;
    }
}
