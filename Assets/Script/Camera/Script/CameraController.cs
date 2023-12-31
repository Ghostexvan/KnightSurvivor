using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    public CameraData cameraData;
    public float default_distance,
                 default_angle;

    private void Awake() {
        Cursor.lockState = CursorLockMode.Confined;

        cameraData.distance = default_distance;
        cameraData.angle = default_angle;

        player = GameObject.FindGameObjectWithTag("Player");

        FollowingPlayer();
    }

    private void Update() {
        if (GameObject.Find("GameController").GetComponent<GameController>().isPlayerDeath){
            GetComponent<Animator>().Play("Death");
        }
    }

    private void LateUpdate() {
        if (GameObject.Find("GameController").GetComponent<GameController>().isPlayerDeath)
            return;
        FollowingPlayer();
    }

    void FollowingPlayer(){
        transform.position = new Vector3(player.transform.position.x, 
                                         player.transform.position.y + cameraData.GetDistanceAxisY(),
                                         player.transform.position.z - cameraData.GetDistanceAxisZ());
        transform.LookAt(player.transform);
    }

    public void ResetCamera(){
        cameraData.distance = default_distance;
        cameraData.angle = default_angle;
    }
}
