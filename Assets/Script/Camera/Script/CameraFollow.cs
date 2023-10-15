using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject player;
    

    private void Awake() {
        player = GameObject.FindGameObjectWithTag("Player");

        FollowingPlayer();
    }

    private void LateUpdate() {
        FollowingPlayer();
    }

    void FollowingPlayer(){
        transform.position = new Vector3(player.transform.position.x, 
                                         gameObject.transform.position.y,
                                         player.transform.position.z);
        transform.LookAt(player.transform);
    }
}
