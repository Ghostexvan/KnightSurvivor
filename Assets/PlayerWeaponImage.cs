using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerWeaponImage : MonoBehaviour
{
    GameObject player;
    private RawImage image;

    private void Awake() {
        player = GameObject.FindGameObjectWithTag("Player");
        image = gameObject.GetComponent<RawImage>();
    }

    private void FixedUpdate() {
        image.texture = player.GetComponent<Equipment>().weapon.itemType.icon.texture;
    }
}
