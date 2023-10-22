using UnityEngine;

public class CameraAnimationController : MonoBehaviour
{
    private void Awake() {
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void Update() {
        if (GameObject.Find("GameController").GetComponent<GameController>().isPlayerDeath){
            GetComponent<Animator>().Play("Death");
        }
    }
}
