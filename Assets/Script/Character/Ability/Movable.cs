using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;

[SelectionBase]
public class Movable : MonoBehaviour
{
    public bool firstGroundContact;
    public CharacterData characterData;
    public float rotateSpeed;
    public Vector2 moveVector,
                   lookVector;
    public bool isActive;
    private void Awake() {
        firstGroundContact = true;
    }
    private void FixedUpdate() {
        if (GetComponent<Health>().health <= 0){
            moveVector = Vector2.zero;
            return;
        }
        
        characterData.isOnGround = IsOnGround();
        characterData.isMove = moveVector != Vector2.zero;
        characterData.moveDirection = GetMoveDirection(Vector2.SignedAngle(moveVector, lookVector));
        //Debug.Log(GetMoveDirection(Vector2.SignedAngle(moveVector, lookVector)));

        //print("Move Vector: " + moveVector);

        // This is the real movement method
        Move();
    }

    private void LateUpdate() {
        Rotate();
    }

    private bool IsOnGround()
    {
        if(firstGroundContact){
            AstarPath.active.Scan();
            firstGroundContact=false;
        }
        return Physics.Raycast(transform.position, Vector3.down, 0.1f);
    }

    private void Move(){
        //if (moveVector != Vector2.zero)
            //print("Move() Is Called");
        transform.Translate(characterData.moveSpeed.Value * new Vector3(moveVector.x, 0f, moveVector.y) * Time.deltaTime, Space.World);
    }

    private void Rotate(){
        // if (characterData.isAttack){
        //     return;
        // }

        if (lookVector == Vector2.zero && moveVector != Vector2.zero)
            lookVector = moveVector;
            
        if (lookVector != Vector2.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation,
                                                  Quaternion.LookRotation(new Vector3(lookVector.x, 0, lookVector.y)),
                                                  Time.deltaTime * rotateSpeed);
        }  
    }

    public void SetMoveVector(Vector2 vec){
        moveVector = vec;
        print("--------- Move Vector set");
        if (vec == Vector2.zero)
            print("Zero");
    }

    public void SetLookVector(Vector2 vec){
        lookVector = vec;
    }

    // Mostly for animation States I think
    Direction GetMoveDirection(float angle){
        if (-22.5f < angle && angle < 22.5f)
            return Direction.Forward;

        if ((-180f < angle && angle < - 157.5f) ||
            (157.5f < angle && angle < 180f))
            return Direction.Backward;
        
        if (22.5f < angle && angle < 67.5f)
            return Direction.ForwardRight;

        if (-67.5f < angle && angle < -22.5f)
            return Direction.ForwardLeft;

        if (67.5f < angle && angle < 112.5f)
            return Direction.Right;

        if (-112.5f < angle && angle < -67.5f)
            return Direction.Left;

        if (112.5f < angle && angle < 157.5f)
            return Direction.BackwardLeft;

        return Direction.BackwardRight;
    }

    public void Active(){
        isActive = true;
    }

    public void Deactive(){
        isActive = false;
    }

    public void SetData(CharacterData data){
        characterData = data;
    }
}
