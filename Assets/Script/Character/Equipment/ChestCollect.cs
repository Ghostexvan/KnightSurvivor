using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;

public class ChestCollect : MonoBehaviour
{
    [Tooltip("List of Items that CAN be dropped from the chest.")]
    public DropableItems data;
    [Tooltip("Items CURRENTLY in the chest. You don't need to modify its data in the Inspector since the whole process is automated.")]
    public List<ItemInstance> itemInstances = new List<ItemInstance>();
    public int itemCount;
    public float rotateSpeed;
    public Vector3 iconRotation;

    private void Awake() {
        iconRotation = gameObject.transform.GetChild(0).rotation.eulerAngles;
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
        gameObject.transform.GetChild(0).eulerAngles = iconRotation;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player"){
            GetDropItems(other.gameObject);
            GameObject.Find("GameCanvas").GetComponent<GameUIController>().selectItemPanel.SetActive(true);
            GameObject.Find("GameCanvas").GetComponent<GameUIController>().selectItemPanel.GetComponent<SelectItemPanel>().SetPlayer(other.gameObject);
            GameObject.Find("GameCanvas").GetComponent<GameUIController>().selectItemPanel.GetComponent<SelectItemPanel>().SetItemList(itemInstances);
            Debug.Log("Done setting to canvas!");
            Destroy(gameObject);
        }
    }

    public void GetDropItems(GameObject character){
        List<ItemData> dropList = new List<ItemData>();

        List<ItemInstance> currentItems = new List<ItemInstance>(/*character.GetComponent<Equipment>().item*/);
        foreach(ItemInstance item in character.GetComponent<Equipment>().item){
            Debug.Log("Item in current: " + item.itemType.name + " - " + item.currentLevel);
            currentItems.Add(new ItemInstance(item.itemType, item.currentLevel));
        }
        Debug.Log("Curent: " + currentItems.Count);
        int itemNumber = currentItems.Count;
        Debug.Log(itemNumber);
        
        // Bien nay dung de kiem tra so luong vu khi co the lay
        // Neu da co vu khi tren tay thi khong the lay qua n-1 vu khi (khong ep nguoi choi doi vu khi)
        int numberWeaponsGet = itemCount;
        if (character.GetComponent<Equipment>().weapon.itemType != null){
            currentItems.Add(new ItemInstance(character.GetComponent<Equipment>().weapon));
            numberWeaponsGet -= 1;
        }
        Debug.Log("Curent after add weapon: " + currentItems.Count);

        List<ItemData> currentItemsData = new List<ItemData>();
        for (int index = 0; index < currentItems.Count; index++)
            currentItemsData.Add(currentItems[index].itemType);

        // Them phan xu ly truong hop so luong item co the drop nho hon so luong can drop
        int numberGetCurrentItem = data.itemLists.Count > itemCount 
                                   ? (itemNumber < 5 ? UnityEngine.Random.Range(0, Math.Min(itemCount, currentItems.Count)) : 3)
                                   : Math.Min(currentItems.Count, data.itemLists.Count);
        Debug.Log("Number of item to get from current list: " + numberGetCurrentItem);

        // Khong xet so luong vu khi co the lay trong kho do cua nguoi choi
        currentItems.Shuffle();
        int getIndex = 0;
        while(numberGetCurrentItem > 0 && getIndex < currentItems.Count){
            if (currentItems[getIndex].currentLevel < currentItems[getIndex].GetMaxLevel()){
                Debug.Log("Add item from current list: " + currentItems[getIndex].itemType.name + " (" + currentItems[getIndex].currentLevel + ", " + currentItems[getIndex].GetMaxLevel() + ")");
                dropList.Add(currentItems[getIndex].itemType);
                numberGetCurrentItem -= 1;
            }
            getIndex++;
        }
        Debug.Log(numberGetCurrentItem + " - " + getIndex);

        List<ItemData> avaiableList = new List<ItemData>(data.itemLists);
        avaiableList.Shuffle();
        int numberToGet = itemCount - dropList.Count;
        Debug.Log(numberToGet);
        getIndex = 0;
        while(numberToGet > 0 && getIndex < avaiableList.Count){
            if (itemNumber == 5 && avaiableList[getIndex].GetType().Name == "ItemData"){
                Debug.Log("Skip");
                getIndex++;
                continue;
            }
                
            if (!dropList.Contains(avaiableList[getIndex]) && !currentItemsData.Contains(avaiableList[getIndex])){
                // Kiem tra xem co phai vu khi khong
                if (avaiableList[getIndex].GetType().Name == "WeaponData"){
                    // So luong vu khi co the lay kha dung thi lay
                    if (numberWeaponsGet > 0)
                        numberWeaponsGet -= 1;
                    else {
                        // Khong thi bo qua
                        getIndex++;
                        continue;
                    }
                }
                
                Debug.Log("Add item from avaiable list: " + avaiableList[getIndex].name);
                dropList.Add(avaiableList[getIndex]);
                numberToGet -= 1;
            }

            getIndex++;
        }
        Debug.Log(numberToGet + " - " + getIndex);

        // Truong hop sau khi lay xong ma chua du, lay nhung effect ngau nhien
        data.effectLists.Shuffle();
        getIndex = 0;
        while(dropList.Count < itemCount){
            dropList.Add(data.effectLists[getIndex]);
            getIndex++;
        }

        for (int index = 0; index < dropList.Count; index++){
            itemInstances.Add(new ItemInstance(dropList[index]));
        }
        currentItems.Clear();
        currentItemsData.Clear();
        dropList.Clear();
        Debug.Log("--------------");
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