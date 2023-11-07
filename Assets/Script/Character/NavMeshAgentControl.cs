using UnityEngine;
using UnityEngine.AI;

public class NavMeshAgentControl : MonoBehaviour
{
    // DataControl.cs has a SendMsg that sets
    public CharacterData characterData;

    private float enemySpeed;

    private void Awake()
    {
        enemySpeed = characterData.moveSpeed.Value;
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        agent.speed = enemySpeed;
    }

    // Start is called before the first frame update
    void FixedUpdate()
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        agent.destination = GameObject.FindGameObjectWithTag("Player").transform.position;       
    }

    public void SetData(CharacterData data)
    {
        characterData = data;
    }
}
