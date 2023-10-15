using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    public GameSettings gameSettings;

    private void Start() {
        gameObject.GetComponent<Slider>().value = gameSettings.volume;
    }

    public void OnValueChange(){
        gameSettings.volume = gameObject.GetComponent<Slider>().value;
        gameSettings.isSet = true;
    }
}
