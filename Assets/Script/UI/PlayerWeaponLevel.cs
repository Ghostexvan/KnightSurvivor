using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerWeaponLevel : MonoBehaviour
{
    GameObject player;
    TMP_Text text;

    private void Awake() {
        player = GameObject.FindGameObjectWithTag("Player");
        text = gameObject.GetComponent<TMP_Text>();
    }

    private void FixedUpdate() {
        if (player.GetComponent<Equipment>().weapon.currentLevel != 0)
            text.text = player.GetComponent<Equipment>().weapon.currentLevel.ToString();
        else
            text.text = "";
    }
}
