using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class debug : MonoBehaviour
{
    public InputActionReference inputAction;
    public int index;

    // Update is called once per frame
    void Update()
    {
        GetComponent<TMP_Text>().text = inputAction.action.GetBindingDisplayString(index);
    }
}
