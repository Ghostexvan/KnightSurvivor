using TMPro;
using UnityEngine;

public class DebugAttack : MonoBehaviour
{
    public GameObject player;
    public TMP_Text text;
    
    private void Awake() {
        player = GameObject.FindGameObjectWithTag("Player");
        text = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = player.GetComponent<Attackable>().isCooldown.ToString();
    }
}
