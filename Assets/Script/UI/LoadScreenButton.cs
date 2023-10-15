using System.Collections;
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
            Debug.Log("Load async");
            StartCoroutine(LoadAsyncScene());
        }
        else
            SceneManager.LoadScene(sceneName);
        isClick = true;
    }

    private void Update() {
        Debug.Log(loadingOperation);
        // if (isAsync && isClick){
        //     loadingPanel.transform.GetChild(0).GetComponent<Slider>().value = Mathf.Clamp01(loadingOperation.progress) / 0.9f;
        // }
    }

    IEnumerator LoadAsyncScene(){
        loadingOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        loadingOperation.allowSceneActivation = false;
        loadingPanel.SetActive(true);

        while (!loadingOperation.isDone){
            loadingPanel.transform.GetChild(0).GetComponent<Slider>().value = Mathf.Clamp01(loadingOperation.progress) / 0.9f;
            Debug.Log("Slider value: " + Mathf.Clamp01(loadingOperation.progress) / 0.9f + " - " + loadingPanel.transform.GetChild(0).GetComponent<Slider>().value);
            
            if (loadingOperation.progress >= 0.9f){
                yield return new WaitForSeconds(2.0f);
                loadingOperation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
