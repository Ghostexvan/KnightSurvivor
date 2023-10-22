using UnityEngine;
using UnityEngine.AI;

public class NavMeshAgentControl : MonoBehaviour
{
    public GameObject target;

    private void Awake() {
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
}
