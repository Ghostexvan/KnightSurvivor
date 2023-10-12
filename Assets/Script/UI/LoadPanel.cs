using UnityEngine;

public class LoadPanel : MonoBehaviour
{
    public GameObject panelToLoad;

    public void OnClick(){
        transform.parent.gameObject.SetActive(false);
        GameObject.Find("Main Camera").GetComponent<Animator>().SetTrigger(panelToLoad.name);
        panelToLoad.SetActive(true);
    }
}
