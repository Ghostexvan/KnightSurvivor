// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.InputSystem;

// public class PlayerController : MonoBehaviour
// {
//     public PlayerData playerData;
//     public int default_health;
//     public float default_moveSpeed,
//                  default_rotateSpeed,
//                  default_jumpForce,
//                  default_dashForce,
//                  default_cooldownTime;
//     public GameObject bullet,
//                       bulletPosition;
//     private Rigidbody rigid;
//     private Animator ani;

//     private void Awake() {
//         rigid = this.gameObject.GetComponent<Rigidbody>();
//         ani = this.gameObject.GetComponent<Animator>();
//         playerData.health = default_health;
//         playerData.moveSpeed = default_moveSpeed;
//         playerData.rotateSpeed = default_rotateSpeed;
//         playerData.jumpForce = default_jumpForce;
//         playerData.dashForce = default_dashForce;
//         playerData.cooldownTime = default_cooldownTime;

//         playerData.isAttack = false;
//     }

//     // Start is called before the first frame update
//     void Start()
//     {
        
//     }

//     // Update is called once per frame
//     void Update()
//     {
        
//     }

//     private void FixedUpdate() {
//         Move();
//         Attack();
//     }

//     private void LateUpdate() {
//         Rotate();
//     }

//     private bool IsGrounded()
//     {
//         return Physics.Raycast(this.transform.position, Vector3.down, 0.1f);
//     }

//     private void Move(){
//         if (IsGrounded() && rigid.velocity.y <= 0 && !playerData.isAttack)
//             ani.Play("Move");

//         if (playerData.moveVector != Vector3.zero)
//             ani.SetFloat("Speed", 1f);
//         else 
//             ani.SetFloat("Speed", 0f);

//         ani.SetFloat("VerticalForce", rigid.velocity.y >= 0 ? 1f : 0f);
//         transform.Translate(playerData.GetTranslateVector(),
//                             Space.World);
//     }

//     private void Jump(){
//         if (!IsGrounded())
//             return;

//         rigid.velocity = new Vector3(0,
//                                      playerData.jumpForce,
//                                      0);
//         ani.Play("Jump");
//     }

//     private void Attack(){
//         if (!playerData.isAttack || playerData.isCooldown)
//             return;

//         if (Time.realtimeSinceStartup - playerData.timeSinceLastAttack >= playerData.cooldownTime){
//             playerData.isAttack = false;
//         }
        

//         playerData.moveVector = Vector3.zero;
//         if (ani.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f)
//             ani.SetFloat("AttackSpeed", 0.0f);

//         //StartCoroutine(Shooting());
//     }

//     IEnumerator Shooting(){
//         Instantiate(bullet, bulletPosition.transform.position, transform.rotation);
//         playerData.isCooldown = true;
//         yield return new WaitForSeconds(playerData.cooldownTime);
//         playerData.isCooldown = false;
//     }


//     private void Rotate(){
//         if (playerData.moveVector != Vector3.zero){
//             transform.rotation = Quaternion.Slerp(transform.rotation,
//                                                   Quaternion.LookRotation(playerData.moveVector),
//                                                   playerData.GetRotationPerFrame());
//         }
//     }

//     public void OnMove(InputAction.CallbackContext context){
//         if (playerData.isAttack){
//             playerData.moveVector = Vector3.zero;
//             return;
//         }
            
//         playerData.moveVector = new Vector3(context.ReadValue<Vector2>().x,
//                                             0,
//                                             context.ReadValue<Vector2>().y);
//     }

//     public void OnJump(InputAction.CallbackContext context){
//         Jump();
//     }

//     public void OnAttack(InputAction.CallbackContext context){
//         if (!IsGrounded())
//             return;

//         playerData.isAttack = true;
//         playerData.timeSinceLastAttack = Time.realtimeSinceStartup;
//         ani.Play("Attack");
//         ani.SetFloat("AttackSpeed", 1.0f);
//     }

//     public void AddDamage(int damage){
//         playerData.health -= damage;
        
//         if (playerData.health <= 0){
//             Destroy(this.gameObject);
//         }
//     }
// }
