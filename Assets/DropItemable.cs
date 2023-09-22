using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DropItemable : MonoBehaviour
{
    [Serializable]
    public struct dropList{
        public GameObject dropItem;
        public float chance;
    }

    public List<dropList> drops;

    private void OnDestroy() {
        Debug.Log("Instantiate position: " + gameObject.transform.position + ", local: " + gameObject.transform.localPosition);
        DropItem();
    }
    
    public void DropItem(){
        Vector3 position = new Vector3(gameObject.transform.localPosition.x,
                                           gameObject.transform.localPosition.y,
                                           gameObject.transform.localPosition.z);
        foreach(dropList drop in drops){
            if (UnityEngine.Random.Range(0f, 100f) <= drop.chance){
                GameObject item = Instantiate(drop.dropItem, position + new Vector3(UnityEngine.Random.Range(-2f, 2f), 0, UnityEngine.Random.Range(-2f, 2f)), Quaternion.identity, GameObject.Find("Collectable").transform);
                //item.transform.position = gameObject.transform.position + Vector3.up*3;
            }
        }
    }
}
