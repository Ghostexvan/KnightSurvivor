using UnityEngine;
using UnityEngine.Events;

public class Destroyable : MonoBehaviour
{
    public UnityEvent onDestroy;
    public bool isActive;

    public void DestroyObject(){
        if (!isActive)
            return;

        onDestroy.Invoke();
        Destroy(gameObject, 0.1f);
    }

    public void Active(){
        isActive = true;
    }

    public void Deactive(){
        isActive = false;
    }
}
