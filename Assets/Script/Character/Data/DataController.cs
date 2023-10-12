using UnityEngine;

public class DataController : MonoBehaviour
{
    public CharacterData data;

    private void Awake() {
        gameObject.BroadcastMessage("SetData", data);
    }
}
