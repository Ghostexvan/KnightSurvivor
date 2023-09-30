using UnityEngine;

public class GameUIController : MonoBehaviour
{
    public GameObject selectItem;

    private void Awake() {
        selectItem = GameObject.Find("SelectItemPanel");
    }
}
