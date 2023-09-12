using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Destroyable : MonoBehaviour
{
    public UnityEvent onDestroy;
    public bool isActive;

    public void DestroyObject(){
        onDestroy.Invoke();
        Destroy(this.gameObject);
    }

    public void Active(){
        isActive = true;
    }

    public void Deactive(){
        isActive = false;
    }
}
