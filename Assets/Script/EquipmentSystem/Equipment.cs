using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    public int maxItem = 5;
    public CharacterData characterData;
    public ItemInstance weapon;
    public List<ItemInstance> item;
    public GameObject itemJoin;
    public Animator ani;
    public RuntimeAnimatorController unarmed;
    public AnimatorOverrideController axe2Hand;
    public AnimatorOverrideController mace1Hand;
    
    private void Awake() {
        itemJoin = gameObject.GetComponent<Attackable>().itemHand;
        ani = gameObject.GetComponent<Animator>();
    }

    private void FixedUpdate() {
        DisplayWeapon();
        ChangeWeaponAnimation();
        gameObject.GetComponent<Animator>().SetInteger("WeaponLevel", weapon.currentLevel);
    }

    public void Equip(ItemInstance item){
        switch (item.itemType.GetType().Name){
                case "WeaponData":
                    if (weapon != null && weapon.itemType != item.itemType){
                        weapon.Unequip(characterData);
                        weapon = item;
                        weapon.Equip(characterData);
                    }
                    else if (weapon != null && weapon.itemType == item.itemType){
                        weapon.LevelUp(characterData);
                    }
                    else {
                        weapon = item;
                        weapon.Equip(characterData);
                    }
                    break;
                case "ItemData":
                    if (this.item.Count < maxItem){
                        for (int index = 0; index < this.item.Count; index++){
                            if (this.item[index].itemType == item.itemType)
                                this.item[index].LevelUp(characterData);
                                return;
                        }
                        item.Equip(characterData);
                        this.item.Add(item);
                    }
                    break;
                default:
                    break;
            }  
    }

    public void DisplayWeapon(){
        for (int index = 0; index < itemJoin.transform.childCount; index++){
            if (weapon != null && itemJoin.transform.GetChild(index).gameObject.name == weapon.itemType.name){
                itemJoin.transform.GetChild(index).gameObject.SetActive(true);
            }
            else if (weapon == null && itemJoin.transform.GetChild(index).gameObject.name == "Unarmed"){
                itemJoin.transform.GetChild(index).gameObject.SetActive(true);
            }
            else
                itemJoin.transform.GetChild(index).gameObject.SetActive(false);
        }
    }

    public void ChangeWeaponAnimation(){
        WeaponData weaponData = (WeaponData) weapon.itemType;
        switch (weaponData.weaponType){
            case WeaponType.Axe:
                switch (weaponData.handleType){
                    case HandleType.TwoHand:
                        ani.runtimeAnimatorController = axe2Hand;
                        break;
                }
                break;
            case WeaponType.Mace:
                switch (weaponData.handleType){
                    case HandleType.OneHand:
                        ani.runtimeAnimatorController = mace1Hand;
                        break;
                }
                break;
            default:
                ani.runtimeAnimatorController = unarmed;
                break;
        }
    }

    public void RemoveItem(ItemInstance item){

    }

    public void RemoveWeapon(){
        if (weapon == null)
            return;

        weapon = null;
        Debug.Log("Remove Complete");
    }

    public void SetData(CharacterData data){
        characterData = data;
    }

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.TryGetComponent(out CollectableItem item)){
            switch (item.itemInstance.itemType.GetType().Name){
                case "WeaponData":
                    if (weapon != null && weapon.itemType != item.itemInstance.itemType){
                        weapon.Unequip(characterData);
                        weapon = item.Collect();
                        weapon.Equip(characterData);
                    }
                    else if (weapon != null && weapon.itemType == item.itemInstance.itemType){
                        item.Collect();
                        weapon.LevelUp(characterData);
                    }
                    else {
                        weapon = item.Collect();
                        weapon.Equip(characterData);
                    }
                    break;
                case "ItemData":
                    if (this.item.Count < maxItem){
                        for (int index = 0; index < this.item.Count; index++){
                            if (this.item[index].itemType == item.itemInstance.itemType)
                                this.item[index].LevelUp(characterData);
                                item.Collect();
                                return;
                        }
                        item.itemInstance.Equip(characterData);
                        this.item.Add(item.Collect());
                    }
                    break;
                default:
                    break;
            }
            
        }
    }

    private void OnApplicationQuit() {
        weapon.Unequip(characterData);

        for (int index = 0; index < item.Count; index++){
            item[index].Unequip(characterData);
        }
    }
}
