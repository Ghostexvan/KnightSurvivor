using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject selectItem;

    private void Awake() {
        selectItem = GameObject.Find("SelectItemPanel");
    }
}
