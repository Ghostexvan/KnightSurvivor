using UnityEngine;

public class CameraFollow_Distance : MonoBehaviour
{
    public GameObject player;
    public CameraData cameraData;
    public float default_distance;

    private void Awake() {
        cameraData.distance = default_distance;

        player = GameObject.FindGameObjectWithTag("Player");

        FollowingPlayer();
    }

    private void Update() {
        if (GameObject.Find("GameController").GetComponent<GameController>().isPlayerDeath)
            return;
        FollowingPlayer();
    }

    void FollowingPlayer(){
        transform.position = new Vector3(player.transform.position.x, 
                                         player.transform.position.y + cameraData.GetDistanceAxisY(),
                                         player.transform.position.z - cameraData.GetDistanceAxisZ());
    }

    public void ResetCamera(){
        cameraData.distance = default_distance;
    }
}
