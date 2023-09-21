using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;

public class ChestCollect : MonoBehaviour
{
    public DropableItems data;
    public List<ItemInstance> itemInstances = new List<ItemInstance>();
    public int itemCount;
    public float rotateSpeed;

    private void Awake() {
        GetDropItems();
    }

    private void FixedUpdate() {
        if (IsOnGround()){
            gameObject.GetComponent<Rigidbody>().isKinematic = true;
        }
        else
            gameObject.GetComponent<Rigidbody>().isKinematic = false;
    }
    
    private void LateUpdate() {
        Rotate();
    }

    void GetItemList(List<ItemData> itemList){
        for (int index = 0; index < itemList.Count; index++){
            Debug.Log("Start create item instance " + index);
            ItemInstance temp = new ItemInstance(itemList[index]);
            Debug.Log("Finish create item instance " + index);
            itemInstances.Add(temp);
        }
    }

    private bool IsOnGround()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1f);
    }

    private void Rotate(){
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player"){
            GameObject.Find("GameCanvas").GetComponent<GameUIController>().selectItem.SetActive(true);
            GameObject.Find("GameCanvas").GetComponent<GameUIController>().selectItem.GetComponent<SelectItemPanel>().SetPlayer(other.gameObject);
            GameObject.Find("GameCanvas").GetComponent<GameUIController>().selectItem.GetComponent<SelectItemPanel>().SetItemList(itemInstances);
            Destroy(gameObject);
        }
    }

    public void GetDropItems(){
        List<ItemData> temp = data.itemLists;
        temp.Shuffle();

        for (int index = 0; index < itemCount; index++){
            itemInstances.Add(new ItemInstance(temp[index]));
        }
    }
}

static class MyExtensions{
    public static void Shuffle<T>(this IList<T> list)
    {
      int n = list.Count;
      while (n > 1)
      {
        n--;
        int k = ThreadSafeRandom.ThisThreadsRandom.Next(n + 1);
        T value = list[k];
        list[k] = list[n];
        list[n] = value;
      }
    }
}

  public static class ThreadSafeRandom
  {
      [ThreadStatic] private static System.Random Local;

      public static System.Random ThisThreadsRandom
      {
          get { return Local ?? (Local = new System.Random(unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId))); }
      }
  }