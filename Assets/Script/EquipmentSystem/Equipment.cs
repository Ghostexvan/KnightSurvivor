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
    
    private void Awake() {
        itemJoinRight = gameObject.transform.Find("Armature/Root_M/Spine1_M/Spine2_M/Chest_M/Scapula_R/Shoulder_R/Elbow_R/Wrist_R/jointItemR").gameObject;
        itemJoinLeft = gameObject.transform.Find("Armature/Root_M/Spine1_M/Spine2_M/Chest_M/Scapula_L/Shoulder_L/Elbow_L/Wrist_L/jointItemL").gameObject;
        ani = gameObject.GetComponent<Animator>();
        weaponInstance = itemJoinRight.transform.GetChild(0).gameObject;
    }

    private void FixedUpdate() {
        //gameObject.GetComponent<Animator>().SetInteger("WeaponLevel", weapon.currentLevel);
    }

    public void Equip(ItemInstance item){
        switch (item.itemType.GetType().Name){
                case "WeaponData":
                    if (weapon != null && weapon.itemType != item.itemType){
                        weapon.Unequip(characterData);
                        if (weaponInstance != itemJoinRight.transform.GetChild(0).gameObject)
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
                    for (int index = 0; index < this.item.Count; index++){
                        if (this.item[index].itemType == item.itemType){
                            this.item[index].LevelUp(characterData);
                            break;
                        }
                    }

                    if (this.item.Count < maxItem){
                        item.Equip(characterData);
                        this.item.Add(item);
                    }
                    
                    break;
                default:
                    break;
            }  
    }

    public void DisplayWeapon(){
        if (weapon != null){
            GameObject target;
            switch (((WeaponData) weapon.itemType).handleType){
                case HandleType.LeftHand:
                    target = itemJoinLeft;
                    break;
                default:
                    target = itemJoinRight;
                    break;
            }

            itemJoinRight.transform.GetChild(0).gameObject.SetActive(false);
            weaponInstance = Instantiate(((WeaponData) weapon.itemType).model, target.transform.position, Quaternion.identity, target.transform);
            weaponInstance.transform.localEulerAngles = Vector3.zero;
            ChangeWeaponAnimation(((WeaponData) weapon.itemType).animator);
        }
        else{
            ChangeWeaponAnimation((AnimatorOverrideController) unarmed);
            itemJoinRight.transform.GetChild(0).gameObject.SetActive(true);
            weaponInstance = itemJoinRight.transform.GetChild(0).gameObject;
        }
            
    }

    public void ChangeWeaponAnimation(AnimatorOverrideController animation){
        ani.runtimeAnimatorController = animation;
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
