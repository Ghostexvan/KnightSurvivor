using UnityEngine;

public class DisplayOnMenu : MonoBehaviour
{
    public ItemData itemData;

    private void Awake() {
        if (PlayerPrefs.HasKey(itemData.name))
            itemData.isGet = PlayerPrefs.GetInt(itemData.name) == 1;

        if (!itemData.isGet)
            gameObject.SetActive(false);
    }

    private void OnApplicationQuit() {
        PlayerPrefs.SetInt(itemData.name, itemData.isGet ? 1 : 0);
    }
}
