using UnityEngine;

public class LoadingBehaviour : MonoBehaviour
{
    public bool isSet,
                isDisplayOnStart,
                isStarted;
    public GameObject mainCamera;

    private void Awake() {
        mainCamera = GameObject.Find("Main Camera");
    }

    private void Start() {
        if(isDisplayOnStart == false){
            Debug.Log("Disable on start: " + gameObject.name + ", " + isDisplayOnStart);
            gameObject.SetActive(false);
        }
        isStarted = true;
    }

    private void Update() {
        if (isSet && !ContainsParams(mainCamera.GetComponent<Animator>(), gameObject.name)){
            for(int index = 0; index < gameObject.transform.childCount; index++){
                gameObject.transform.GetChild(index).gameObject.SetActive(true);
            }
            isSet = false;
        }

        //Debug.Log(mainCamera.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsTag(gameObject.name));
        if (isSet && mainCamera.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsTag(gameObject.name) && mainCamera.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f){
            for(int index = 0; index < gameObject.transform.childCount; index++){
                gameObject.transform.GetChild(index).gameObject.SetActive(true);
            }
            isSet = false;
            //mainCamera.gameObject.GetComponent<Animator>().SetBool(gameObject.name, false);
        }
    }

    public bool ContainsParams(Animator animator, string paramName){
        foreach (AnimatorControllerParameter param in animator.parameters){
            if (param.name == paramName)
                return true;
        }
        return false;
    }

    private void OnEnable() {
        if (!isStarted)
            return;
            
        // if(isDisplayOnStart){
        //     isDisplayOnStart = false;
        //     return;
        // }
        
        for(int index = 0; index < gameObject.transform.childCount; index++){
            gameObject.transform.GetChild(index).gameObject.SetActive(false);
        }
        isSet = true;
    }
}
