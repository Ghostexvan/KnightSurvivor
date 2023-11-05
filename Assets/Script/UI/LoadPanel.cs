using UnityEngine;

public class LoadPanel : MonoBehaviour
{
    public GameObject panelToLoad,
                      parentPanel;
    public bool useCustomePanel;

    public void OnClick(){
        if (useCustomePanel){
            parentPanel.SetActive(false);
        }
        else {
            transform.parent.gameObject.SetActive(false);
        }
        GameObject.Find("Main Camera").GetComponent<Animator>().SetTrigger(panelToLoad.name);
        panelToLoad?.SetActive(true);
    }
}
