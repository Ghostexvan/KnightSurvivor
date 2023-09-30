using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    GameObject player;
    Image healthBar;

    private void Awake() {
        player = GameObject.FindGameObjectWithTag("Player");
        healthBar = transform.GetChild(0).gameObject.GetComponent<Image>();
    }

    private void FixedUpdate() {
        healthBar.fillAmount = player.GetComponent<Health>().health / player.GetComponent<Health>().maxHealth;
    }
}
