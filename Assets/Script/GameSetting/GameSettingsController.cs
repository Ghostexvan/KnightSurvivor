using UnityEngine;

public class GameSettingsController : MonoBehaviour
{
    public GameSettings gameSettings;

    private void Awake() {
        DontDestroyOnLoad(gameObject);
        ApplySettings();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (gameSettings.isSet)
            ApplySettings();
    }

    void ApplySettings(){
        FullScreenMode screenMode = gameSettings.resolution.isFullscreen ? FullScreenMode.ExclusiveFullScreen : FullScreenMode.Windowed;
        Screen.SetResolution(gameSettings.resolution.width, gameSettings.resolution.height, screenMode);

        Application.targetFrameRate = gameSettings.frameRate;
        GetComponent<AudioSource>().volume = gameSettings.volume;
        gameSettings.isSet = false;
    }
}
