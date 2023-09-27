using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class Controllable : MonoBehaviour
{
    public UnityEvent<Vector2> onMove,
                               onLook;
    public UnityEvent onJump,
                      onAttack;
    public bool isActive;

    [HideInInspector]
    public GameObject gameController;

    // Dùng để xét nếu UDP có bật hay ko, nếu có thì hàm OnMoveInput sẽ Invoke cả onMove và onLook (vì ta đã disable chuột khi có UDP nên nhân vật sẽ ko rotate đc)
    [Tooltip("Is used to check whether UDP is active or not. If it is, then OnMoveInput will Invoke both onMove and onLook (Because when UDP is active, we disable our mouse input, so our character can't rotate)")]
    private bool isUDPConActive;

    private void Awake()
    {
        gameController = GameObject.Find("GameController");
    }

    private void Start()
    {
        isUDPConActive = gameController.GetComponent<UDPControllable>().isActive;
    }

    public void OnMoveInput(InputAction.CallbackContext context){
        if (!isActive){
            return;
        }

        print("----- On Move Input");
        onMove.Invoke(context.ReadValue<Vector2>());
        if (context.ReadValue<Vector2>() == Vector2.zero)
        {
            print("V2 Zero is read");
        }

        if (isUDPConActive == true)
        {
            onLook.Invoke(context.ReadValue<Vector2>());
        }
    }

    //public void OnHoldInteraction(InputAction.CallbackContext context)
    //{
    //    switch(context.phase)
    //    {
    //        case InputActionPhase.Started:
    //            print(context.interaction + "--- Started");
    //            break;
    //        case InputActionPhase.Performed:
    //            print(context.interaction + "--- Performed");
    //            break;
    //        case InputActionPhase.Canceled:
    //            print(context.interaction + "--- Cancelled");
    //            break;

    //        default:
    //            break;
    //    }
    //}

    public void OnJumpInput(InputAction.CallbackContext context){
        if (!isActive){
            return;
        }

        onJump.Invoke();
    }

    public void OnAttackInput(InputAction.CallbackContext context){
        if (!isActive)
            return;

        onAttack.Invoke();
    }

    public void OnMousePositionInput(InputAction.CallbackContext context){
        Vector2 lookVector = context.ReadValue<Vector2>() - new Vector2(Screen.width / 2, Screen.height / 2);
        onLook.Invoke(new Vector2 (Remap(lookVector.x, -Screen.width / 2, Screen.width / 2, -1, 1),
                                   Remap(lookVector.y, -Screen.width / 2, Screen.width / 2, -1, 1)));
        // Debug.Log(new Vector2 (Remap(lookVector.x, -Screen.width / 2, Screen.width / 2, -1, 1),
        //                        Remap(lookVector.y, -Screen.width / 2, Screen.width / 2, -1, 1)));
    }

    public float Remap (float value, float from1, float to1, float from2, float to2) {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    public void Active(){
        isActive = true;
    }

    public void Deactive(){
        isActive = false;
    }
}
