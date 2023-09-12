using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemDisplay : MonoBehaviour
{
    GameObject player;
    public int index;
    Image image;
    TMP_Text text;

    private void Awake() {
        player = GameObject.FindGameObjectWithTag("Player");
        image = gameObject.GetComponent<Image>();
        text =  gameObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>();
    }

    private void FixedUpdate() {
        if (player.GetComponent<Equipment>().item.Count < index + 1){
            image.sprite = null;
            text.text = "";
            return;
        }

        image.sprite = player.GetComponent<Equipment>().item[index].itemType.icon;
        if (player.GetComponent<Equipment>().item[index].currentLevel == 0)
            text.text = "";
        else
            text.text = player.GetComponent<Equipment>().item[index].currentLevel.ToString();
    }
}
