using UnityEngine;

public class CollectableItem : MonoBehaviour
{
    public ItemInstance itemInstance;
    public float rotateSpeed;

    private void Awake() {
        itemInstance = new ItemInstance(itemInstance.itemType);
    }

    private void FixedUpdate() {
        if (IsOnGround()){
            gameObject.GetComponent<Rigidbody>().isKinematic = true;
        }
        else
            gameObject.GetComponent<Rigidbody>().isKinematic = false;
    }
    
    private void LateUpdate() {
        Rotate();
    }

    public ItemInstance Collect(){
        Destroy(gameObject);
        return itemInstance;
    }

    private bool IsOnGround()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1f);
    }

    private void Rotate(){
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
    }
}