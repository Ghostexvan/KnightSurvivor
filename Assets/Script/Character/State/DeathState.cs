using UnityEngine;

public class DeathState : IState
{
    public void OnEnter(StateController sc)
    {
        sc.onDeath.Invoke();
        sc.ani.Play("Death");
        sc.playerObject.GetComponent<Rigidbody>().isKinematic = true;
        sc.playerObject.GetComponent<BoxCollider>().enabled = false;
        Debug.Log("Death");
    }

    public void OnExit(StateController sc)
    {
        
    }

    public void OnHurt(StateController sc)
    {
        
    }

    public void UpdateState(StateController sc)
    {
        
    }
}
