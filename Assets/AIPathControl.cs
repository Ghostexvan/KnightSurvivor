using Pathfinding;
using UnityEngine;

public class AIPathControl : MonoBehaviour
{
    public CharacterData characterData;
    private float enemySpeed;
    private void Awake()
    {
        enemySpeed = characterData.moveSpeed.Value;
        AIPath ai = GetComponent<AIPath>();
        ai.maxSpeed = enemySpeed;
    }
    public void SetData(CharacterData data)
    {
        characterData = data;
    }
}
