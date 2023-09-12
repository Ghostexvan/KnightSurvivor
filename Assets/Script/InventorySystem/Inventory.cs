// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class Inventory : MonoBehaviour
// {
//     public List<ItemInstance> items = new List<ItemInstance>();
//     public int maxSlots = 10;
    
//     public void AddItem(CollectableItem collectable){
//         Debug.Log(collectable.itemInstance.itemType.GetType().Name);
//         switch(collectable.itemInstance.itemType.GetType().Name){
//             case "WeaponData":
//                 AddWeaponAndArmor(collectable.CollectItem());
//                 break;
//             case "ArmorData":
//                 AddWeaponAndArmor(collectable.CollectItem());
//                 break;
//         }
//     }

//     void AddWeaponAndArmor(ItemInstance item){
//         if(items.Count < maxSlots)
//             items.Add(item);
//     }

//     public void Equip(int index){
//         if (items[index].Equip(gameObject))
//             items.RemoveAt(index);
//     }

//     private void OnCollisionEnter(Collision other) {
//         if (other.gameObject.TryGetComponent(out CollectableItem collectable)){
//             AddItem(collectable);
//         }
//     }
// }
