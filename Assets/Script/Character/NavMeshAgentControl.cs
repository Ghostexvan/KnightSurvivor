using UnityEngine;
using UnityEngine.AI;

public class NavMeshAgentControl : MonoBehaviour
{
    // Start is called before the first frame update
    void FixedUpdate()
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        agent.destination = GameObject.FindGameObjectWithTag("Player").transform.position;       
    }
}
