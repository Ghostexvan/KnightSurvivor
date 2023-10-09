using TMPro;
using UnityEngine;

public class debug : MonoBehaviour
{
    public GameSettings gameSettings;

    // Update is called once per frame
    void Update()
    {
        GetComponent<TMP_Text>().text = gameSettings.resolution.width + " x " + gameSettings.resolution.height + " | " + gameSettings.volume + " | " + gameSettings.frameRate;
    }
}
