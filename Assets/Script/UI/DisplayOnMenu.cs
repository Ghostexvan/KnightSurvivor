using UnityEngine;

public class DisplayOnMenu : MonoBehaviour
{
    public ItemData itemData;

    private void Awake() {
        if (!itemData.isGet)
            gameObject.SetActive(false);
    }
}
