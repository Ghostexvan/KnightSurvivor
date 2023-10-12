using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadScreenButton : MonoBehaviour
{
    public string sceneName;
    public bool isAsync,
                isClick;
    public GameObject loadingPanel;
    AsyncOperation loadingOperation;

    private void Awake() {
        loadingPanel = GameObject.Find("LoadingScreen").gameObject;
        isClick = false;
    }
    
    public void OnClick(){
        if (isAsync){
            loadingOperation = SceneManager.LoadSceneAsync(sceneName);
            loadingPanel.SetActive(true);
        }
        else
            SceneManager.LoadScene(sceneName);
        isClick = true;
    }

    private void Update() {
        if (isAsync && isClick){
            loadingPanel.transform.GetChild(0).GetComponent<Slider>().value = Mathf.Clamp01(loadingOperation.progress) / 0.9f;
        }
    }
}
