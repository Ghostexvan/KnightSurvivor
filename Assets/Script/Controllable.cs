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

    public void OnMoveInput(InputAction.CallbackContext context){
        if (!isActive){
            return;
        }

        onMove.Invoke(context.ReadValue<Vector2>());
    }

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
