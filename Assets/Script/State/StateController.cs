using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
public class StateController : MonoBehaviour
{
    IState currentState;
    public CharacterData playerData;
    public Animator ani;
    public RunState runState = new RunState();
    public JumpState jumpState = new JumpState();
    public FallState fallState = new FallState();
    //public AttackState attackState = new AttackState();

    public float timeBeforeOut,
                 activeTime;

    public UnityEvent onRunStart,
                      onJumpStart,
                      onFallStart,
                      onAttackStart;

    private void Awake() {
        ani = GetComponent<Animator>();
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
