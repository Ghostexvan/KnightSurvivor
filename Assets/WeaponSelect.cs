using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class WeaponSelect : MonoBehaviour, IPointerEnterHandler
{
    public ItemInstance itemInstance;
    public GameObject character;
    public bool isSet;
    public TMP_Text itemName, 
                    description,
                    stat;
    public int index;

    private void Awake() {
        itemName = gameObject.transform.GetChild(1).transform.GetChild(0).gameObject.GetComponent<TMP_Text>();
        description = gameObject.transform.GetChild(1).transform.GetChild(1).gameObject.GetComponent<TMP_Text>();
        stat = gameObject.transform.GetChild(1).transform.GetChild(2).gameObject.GetComponent<TMP_Text>();
    }

    private void Update() {
        if (isSet){
            gameObject.transform.GetChild(0).gameObject.GetComponent<RawImage>().texture = itemInstance.itemType.icon.texture;
            itemName.text = itemInstance.itemType.itemName + " | Level ";
            if (GetLevel() == itemInstance.GetMaxLevel())
                itemName.text += "Max";
            else
                itemName.text += GetLevel();
                
            description.text = itemInstance.itemType.description;
            stat.text = GetDescription();
            isSet = false;
        }
    }

    private int GetWeaponLevel(){
        // The character already equip this weapon
        if (character.GetComponent<Equipment>().weapon.itemType == itemInstance.itemType){
            return character.GetComponent<Equipment>().weapon.currentLevel + 1;
        }
        return 1;
    }

    private int GetItemLevel(){
        // Search through character item list to see if the character already have this item
        for (int index = 0; index < character.GetComponent<Equipment>().item.Count; index++){
            if (itemInstance.itemType == character.GetComponent<Equipment>().item[index].itemType){
                return character.GetComponent<Equipment>().item[index].currentLevel + 1;
            }
        }
        return 1;
    }

    private int GetLevel(){
        if (itemInstance.itemType.GetType().Name == "WeaponData")
            return GetWeaponLevel();
        return GetItemLevel();
    }

    private string GetDescription(){
        string statDescription = "";
        
        int targetLevel= GetLevel();
          
        for (int index = 0; index < itemInstance.itemType.itemStats.Count; index++){
            if (itemInstance.itemType.itemStats[index].level == targetLevel)
            {
                if (statDescription != "")
                    statDescription += "; ";
                statDescription += itemInstance.itemType.itemStats[index].statAffect.ToString();
                statDescription += ": ";

                if (itemInstance.itemType.itemStats[index].statType == StatModType.Flat || itemInstance.itemType.itemStats[index].statType == StatModType.PercentAdd){
                    if (itemInstance.itemType.itemStats[index].value >= 0)
                        statDescription += "+";
                }
                else {
                    statDescription += "x";
                }

                statDescription += itemInstance.itemType.itemStats[index].value;

                if (itemInstance.itemType.itemStats[index].statType == StatModType.PercentAdd || itemInstance.itemType.itemStats[index].statType == StatModType.PercentMult){
                    statDescription += "%";
                }
            }
        }

        return statDescription;
    }

    private void OnEnable() {
        itemInstance = null;
        character = null;
    }

    private void OnDisable() {
        Destroy(gameObject);
    }

    public void OnClick(){
        character.GetComponent<Equipment>().Equip(itemInstance);
        gameObject.transform.parent.transform.parent.gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

    public void SetCharacterData(GameObject character){
        this.character = character;
    }

    public void SetItemInstance(ItemInstance itemInstance){
        this.itemInstance = itemInstance;
    }

    public void HighLight(bool isHighLight){
        if (isHighLight){
            gameObject.GetComponent<Image>().color = Color.white;
        }
        else
            gameObject.GetComponent<Image>().color = Color.black;
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Hover: " + index);
        gameObject.transform.parent.parent.gameObject.GetComponent<SelectItemPanel>().SetCurrentIndex(index);
    }
}
