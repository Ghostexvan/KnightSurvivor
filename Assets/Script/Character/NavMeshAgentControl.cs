using UnityEngine;
using UnityEngine.AI;

public class NavMeshAgentControl : MonoBehaviour
{
    // DataControl.cs has a SendMsg that sets
    public CharacterData characterData;

    private float enemySpeed;
    public GameObject target;

    private void Awake()
    {
        enemySpeed = characterData.moveSpeed.Value;
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        agent.speed = enemySpeed;
        target = GameObject.FindGameObjectWithTag("Player");
    }

    // Start is called before the first frame update
    void FixedUpdate()
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (target.GetComponent<Health>().health > 0){
            agent.destination = target.transform.position;      
        }
        else{
            agent.isStopped = true;
        }
    }

    public void SetData(CharacterData data)
    {
        characterData = data;
    }
}
