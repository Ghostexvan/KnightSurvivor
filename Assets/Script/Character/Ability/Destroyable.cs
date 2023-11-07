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

    /// This was an idea to decrement the current enemy count (on field) in EnemySpawnController
    /// But Destroyable.cs is used for both Player and Enemies so this felt wrong somehow
    /// Don't get me wrong it still works, but it doesn't seem all that ethical tbh.
    //private void OnDestroy()
    //{
    //    if (this.name != "Player" && this.CompareTag("Enemy")) {
    //        EnemySpawnController esc_ = GameObject.Find("Game Controller").GetComponent<EnemySpawnController>();
    //        esc_.OnEnemyKilled();
    //    }
    //}
}
