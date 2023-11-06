using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KillCountUI : MonoBehaviour
{
    private GameObject gameController;
    private int killCount;              // Total Enemy Killcount
    private TMP_Text killCountText;     // Killcount UI

    private void Awake()
    {
        gameController = GameObject.Find("GameController");
        killCountText = this.transform.GetChild(1).GetComponent<TMP_Text>();    // Child idx 1 is "EnemyCount (TMP)"
        // Reason why I find stuffs like this rather than like InputModeUI.cs is because I kinda felt like it.
        // They both aren't really elegant solutions though... But they work so yeah.

        killCount = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        killCount = gameController.GetComponent<EnemySpawnController>().totalEnemiesKilled;
        killCountText.text = killCount.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        killCount = gameController.GetComponent<EnemySpawnController>().totalEnemiesKilled;
        killCountText.text = killCount.ToString();
    }

    private void OnDestroy()
    {
        killCount = 0;
    }

    private void OnApplicationQuit()
    {
        killCount = 0;
    }
}
