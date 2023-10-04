using TMPro;
using UnityEngine;

public class ResolutionDropdown : MonoBehaviour
{
    public GameSettings gameSettings;

    private void Awake() {
        switch (gameSettings.resolution.width){
            case 1920:
                if (gameSettings.resolution.isFullscreen)
                    GetComponent<TMP_Dropdown>().value = 0;
                else
                    GetComponent<TMP_Dropdown>().value = 1;
                break;
            case 1600:
                if (gameSettings.resolution.isFullscreen)
                    GetComponent<TMP_Dropdown>().value = 2;
                else
                    GetComponent<TMP_Dropdown>().value = 3;
                break;
            case 1280:
                if (gameSettings.resolution.isFullscreen)
                    GetComponent<TMP_Dropdown>().value = 4;
                else
                    GetComponent<TMP_Dropdown>().value = 5;
                break;
            case 960:
                if (gameSettings.resolution.isFullscreen)
                    GetComponent<TMP_Dropdown>().value = 6;
                else
                    GetComponent<TMP_Dropdown>().value = 7;
                break;
            case 640:
                if (gameSettings.resolution.isFullscreen)
                    GetComponent<TMP_Dropdown>().value = 8;
                else
                    GetComponent<TMP_Dropdown>().value = 9;
                break;
        }
    }

    public void OnChangeValue(){
        string text = GetComponent<TMP_Dropdown>().options[GetComponent<TMP_Dropdown>().value].text;
        if (text.Contains("Fullscreen"))
            gameSettings.resolution.isFullscreen = true;
        else
            gameSettings.resolution.isFullscreen = false;
        
        string[] resolutionField = text.Split(" ");
        string[] resolutionValue = resolutionField[0].Split("x");
        gameSettings.resolution.width = int.Parse(resolutionValue[0]);
        gameSettings.resolution.height = int.Parse(resolutionValue[1]);
        gameSettings.isSet = true;
    }
}
