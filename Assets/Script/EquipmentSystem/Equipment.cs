using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    public int maxItem = 5;
    public CharacterData characterData;
    public ItemInstance weapon;
    public List<ItemInstance> item;
    public GameObject itemJoinRight,
                      itemJoinLeft,
                      weaponInstance;
    public Animator ani;
    public RuntimeAnimatorController unarmed;
    public AnimatorOverrideController axe2Hand;
    public AnimatorOverrideController mace1Hand;
    
    private void Awake() {
        itemJoinRight = gameObject.transform.Find("Armature/Root_M/Spine1_M/Spine2_M/Chest_M/Scapula_R/Shoulder_R/Elbow_R/Wrist_R/jointItemR").gameObject;
        itemJoinLeft = gameObject.transform.Find("Armature/Root_M/Spine1_M/Spine2_M/Chest_M/Scapula_L/Shoulder_L/Elbow_L/Wrist_L/jointItemL").gameObject;
        ani = gameObject.GetComponent<Animator>();
    }

    private void FixedUpdate() {
        //DisplayWeapon();
        ChangeWeaponAnimation();
        gameObject.GetComponent<Animator>().SetInteger("WeaponLevel", weapon.currentLevel);
    }

    public void Equip(ItemInstance item){
        switch (item.itemType.GetType().Name){
                case "WeaponData":
                    if (weapon != null && weapon.itemType != item.itemType){
                        weapon.Unequip(characterData);
                        Destroy(weaponInstance);
                        weapon = item;
                        weapon.Equip(characterData);
                        DisplayWeapon();
                    }
                    else if (weapon != null && weapon.itemType == item.itemType){
                        weapon.LevelUp(characterData);
                    }
                    else {
                        weapon = item;
                        weapon.Equip(characterData);
                        DisplayWeapon();
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
        weaponInstance = Instantiate(((WeaponData) weapon.itemType).model, itemJoinRight.transform.position, Quaternion.identity, itemJoinRight.transform);
        weaponInstance.transform.localEulerAngles = Vector3.zero;
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

    public void SetData(CharacterData data){
        characterData = data;
    }

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.TryGetComponent(out CollectableItem item)){
            Equip(item.Collect());            
        }
    }

    private void OnApplicationQuit() {
        weapon.Unequip(characterData);

        for (int index = 0; index < item.Count; index++){
            item[index].Unequip(characterData);
        }
    }
}
