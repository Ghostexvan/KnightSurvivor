using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow_Angle : MonoBehaviour
{
    public GameObject player;
    public CameraData cameraData;
    public float default_angle;

    private void Awake() {
        cameraData.angle = default_angle;

        player = GameObject.FindGameObjectWithTag("Player");

        FollowingPlayer();
    }

    private void LateUpdate() {
        if (GameObject.Find("GameController").GetComponent<GameController>().isPlayerDeath)
            return;
        FollowingPlayer();
    }

    void FollowingPlayer(){
        transform.LookAt(player.transform);
    }

    public void ResetCamera(){
        cameraData.angle = default_angle;
    }
}
