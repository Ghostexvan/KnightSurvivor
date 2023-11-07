using UnityEngine;

public class GameUIController : MonoBehaviour
{
    public GameObject selectItemPanel,
                      gamePausePanel;

    private void Awake() {
        selectItemPanel = GameObject.Find("SelectItemPanel");
        gamePausePanel = GameObject.Find("GamePausePanel");
    }

    public void PauseGame(){
        gamePausePanel.SetActive(true);
    }
}
