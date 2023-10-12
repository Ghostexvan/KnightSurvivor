// using UnityEngine;
// using UnityEngine.UI;

// public class buttonControl : MonoBehaviour
// {
//     public GameObject player;

//     private void Awake() {
//         player = GameObject.FindGameObjectWithTag("Player");
//     }

//     private void FixedUpdate() {
//         for (int index = 0; index < transform.childCount; index++){
//             if (index >= player.GetComponent<Inventory>().items.Count){
//                 gameObject.transform.GetChild(index).GetComponent<Image>().sprite = null;
//             }
//             else {
//                 gameObject.transform.GetChild(index).GetComponent<Image>().sprite = player.GetComponent<Inventory>().items[index].itemType.icon;
//             }    
//         }
//     }

//     public void buttonClick(int index){
//         player.GetComponent<Inventory>().Equip(index);
//     }
// }
