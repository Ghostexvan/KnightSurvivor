using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ChangeKeyBinding : MonoBehaviour
{
    public InputActionReference inputAction;
    public InputActionRebindingExtensions.RebindingOperation rebind;
    public int bindingIndex;
    public bool isBinding;
    public float timeBeforeFade,
                 timeStartBlinking;

    // Start is called before the first frame update
    void Start()
    {
        LoadBinding();
        //inputAction.action.ApplyBindingOverride(bindingIndex ,"<Keyboard>/q");
        Debug.Log(inputAction.action.bindings[bindingIndex].name);
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.GetChild(0).GetComponent<TMP_Text>().text = inputAction.action.GetBindingDisplayString(bindingIndex);
    }

    public void OnClick(){
        ChangeShortcut(inputAction.action);
    }

    public void ChangeShortcut(InputAction action){
        Debug.Log("Start rebinding button for action " + inputAction.name + "!");
        StartCoroutine("BlinkingText");
        gameObject.GetComponent<Button>().enabled = false;
        rebind?.Cancel();
        action.Disable();
        rebind = action.PerformInteractiveRebinding(bindingIndex).WithCancelingThrough("<Keyboard>/escape");
        rebind.OnCancel(
            operation => {
                Debug.Log("Cancel rebind button");
                CleanUp(action);
            }
        ).OnComplete(
            operation => {
                Debug.Log("Bind button completed");
                Debug.Log(inputAction.action.bindings[bindingIndex]);
                CleanUp(action);
            }
        ).Start();
    }

    void CleanUp(InputAction action){
        StopCoroutine("BlinkingText");
        gameObject.transform.GetChild(0).GetComponent<TMP_Text>().faceColor = new Color32(gameObject.transform.GetChild(0).GetComponent<TMP_Text>().faceColor.r,
                                                                                              gameObject.transform.GetChild(0).GetComponent<TMP_Text>().faceColor.g,
                                                                                              gameObject.transform.GetChild(0).GetComponent<TMP_Text>().faceColor.b,
                                                                                              255);
        action.Enable();
        rebind?.Dispose();
        rebind = null;
        gameObject.GetComponent<Button>().enabled = true;
        SaveBinding();
    }

    IEnumerator BlinkingText(){
        while (true){
            // if (Time.realtimeSinceStartup >= timeStartBlinking){
            //     timeStartBlinking += timeBeforeFade;
                // gameObject.transform.GetChild(0).GetComponent<TMP_Text>().faceColor = new Color32((byte) gameObject.transform.GetChild(0).GetComponent<TMP_Text>().color.r,
                //                                                                               (byte) gameObject.transform.GetChild(0).GetComponent<TMP_Text>().color.g,
                //                                                                               (byte) gameObject.transform.GetChild(0).GetComponent<TMP_Text>().color.b,
                //                                                                               0);
            //     Debug.Log(gameObject.transform.GetChild(0).GetComponent<TMP_Text>().faceColor);
            // }
            gameObject.transform.GetChild(0).GetComponent<TMP_Text>().faceColor = new Color32(gameObject.transform.GetChild(0).GetComponent<TMP_Text>().faceColor.r,
                                                                                              gameObject.transform.GetChild(0).GetComponent<TMP_Text>().faceColor.g,
                                                                                              gameObject.transform.GetChild(0).GetComponent<TMP_Text>().faceColor.b,
                                                                                              0);
            yield return new WaitForSeconds(0.5f);
            gameObject.transform.GetChild(0).GetComponent<TMP_Text>().faceColor = new Color32(gameObject.transform.GetChild(0).GetComponent<TMP_Text>().faceColor.r,
                                                                                              gameObject.transform.GetChild(0).GetComponent<TMP_Text>().faceColor.g,
                                                                                              gameObject.transform.GetChild(0).GetComponent<TMP_Text>().faceColor.b,
                                                                                              255);
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void LoadBinding(){
        Debug.Log("Saved binding: " + PlayerPrefs.GetString(inputAction.action.bindings[bindingIndex].name));
        if (PlayerPrefs.HasKey(inputAction.action.bindings[bindingIndex].name)){
            inputAction.action.ApplyBindingOverride(bindingIndex, PlayerPrefs.GetString(inputAction.action.bindings[bindingIndex].name));
        }
    }

    public void SaveBinding(){
        Debug.Log("Saved new binding with path: " + inputAction.action.bindings[bindingIndex].overridePath);
        PlayerPrefs.SetString(inputAction.action.bindings[bindingIndex].name, inputAction.action.bindings[bindingIndex].overridePath);
        Debug.Log("Path in PlayerPrefs: " + PlayerPrefs.GetString(inputAction.action.bindings[bindingIndex].name));
    }

    private void OnApplicationQuit() {
        SaveBinding();
    }
}
