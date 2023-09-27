using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InputModeUI : MonoBehaviour
{
    private GameObject gameController;
    private TMP_Text text;
    private GameObject timeCounter;
    private bool isUDPActive;

    private void Awake()
    {
        gameController = GameObject.Find("GameController");
        text = gameObject.transform.Find("Input Mode Text (TMP)").GetComponent<TMP_Text>();       // Input Mode UI Text
        timeCounter = gameObject.transform.Find("Keyboard Input Time (TMP)").gameObject;      // Keyboard Input Waiting Time UI
    }

    // Start is called before the first frame update
    void Start()
    {
        isUDPActive = gameController.GetComponent<UDPControllable>().isActive;
        if (isUDPActive)
        {
            text.text = "UDP Input Mode";
            text.color = new Color32(0, 255, 0, 255);
        }
        else
        {
            text.text = "Keyboard Input Mode";
            text.color = new Color32(255, 0, 0, 255);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        float elapsedKBTime = gameController.GetComponent<UDPControllable>().elapsedKBTime;     // elapsedKBTime == TimeToWait trong UDPControllable.cs

        isUDPActive = gameController.GetComponent<UDPControllable>().isActive;
        if (isUDPActive)
        {
            text.text = "UDP Input Mode";
            text.color = new Color32(0, 255, 0, 255);
            if (gameController.GetComponent<UDPControllable>().isKeyboardInput)
            {
                text.text = "(TEMP) Keyboard Input Mode";
                text.color = new Color32(255, 0, 0, 255);
            }
        }
        else
        {
            text.text = "Keyboard Input Mode";
            text.color = new Color32(255, 0, 0, 255);
        }


        //// Putting countdown here to see things easier, the conds are basically the same
        if (elapsedKBTime > 0 && isUDPActive && gameController.GetComponent<UDPControllable>().isKeyboardInput)
        {
            timeCounter.SetActive(true);
            //elapsedKBTime -= Time.deltaTime;
            timeCounter.GetComponent<TMP_Text>().text = elapsedKBTime.ToString("0.00");
        }
        if (elapsedKBTime <= 0)
        {
            timeCounter.SetActive(false);
        }
    }
}
