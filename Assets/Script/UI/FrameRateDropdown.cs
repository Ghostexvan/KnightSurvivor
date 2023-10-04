using TMPro;
using UnityEngine;

public class FrameRateDropdown : MonoBehaviour
{
    public GameSettings gameSettings;


    private void Awake() {
        switch (gameSettings.frameRate) {
            case -1:
                GetComponent<TMP_Dropdown>().value = 2;
                break;
            case 30:
                GetComponent<TMP_Dropdown>().value = 1;
                break;
            case 60:
                GetComponent<TMP_Dropdown>().value = 0;
                break;
            default:
                break;
        }
    }
    
    public void OnChangeValue(){
        string text = GetComponent<TMP_Dropdown>().options[GetComponent<TMP_Dropdown>().value].text;
        if (text.Contains("No limit")){
            gameSettings.frameRate = -1;
            gameSettings.isSet = true;
            return;
        }
        
        gameSettings.frameRate = int.Parse(text);
        gameSettings.isSet = true;
    }
}
