using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
public class StateController : MonoBehaviour
{
    IState currentState;
    public CharacterData playerData;
    public Health health;
    public GameObject playerObject;
    public Animator ani;
    public RunState runState = new RunState();
    public JumpState jumpState = new JumpState();
    public FallState fallState = new FallState();
    public DeathState deathState = new DeathState();
    //public AttackState attackState = new AttackState();

    public float timeBeforeOut,
                 activeTime;

    public UnityEvent onRunStart,
                      onJumpStart,
                      onFallStart,
                      onAttackStart,
                      onDeath;

    private void Awake() {
        ani = GetComponent<Animator>();
        health = GetComponent<Health>();
        playerObject = gameObject;
    }

    // Start is called before the first frame update
    void Start()
    {
        ChangeState(runState);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (currentState !=  null){
            currentState.UpdateState(this);
            playerData.currentState = currentState.GetType().Name;
        }
    }

    public void ChangeState(IState newState){
        if (currentState != null){
            currentState.OnExit(this);
        }

        currentState = newState;
        currentState.OnEnter(this);
    }

    public void SetData(CharacterData data){
        playerData = data;
    }
}
