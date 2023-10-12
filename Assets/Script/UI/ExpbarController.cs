using UnityEngine;
using UnityEngine.UI;

public class ExpbarController : MonoBehaviour
{
    GameObject player;
    public Image expBar;

    private void Awake() {
        player = GameObject.FindGameObjectWithTag("Player");
        expBar = transform.GetChild(0).gameObject.GetComponent<Image>();
    }

    private void FixedUpdate() {
        expBar.fillAmount = (float) player.GetComponent<CharacterLevel>().currentEXP / player.GetComponent<CharacterLevel>().needEXP;
        //Debug.Log(expBar.fillAmount);
    }
}
