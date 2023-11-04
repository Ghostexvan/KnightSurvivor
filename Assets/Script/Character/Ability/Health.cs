using System;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public CharacterData characterData;
    public float health,
                 maxHealth;
    public UnityEvent onHealthZero;
    public bool isActive;

    private void Awake() {
        maxHealth = characterData.health.Value;
        health = maxHealth;
    }

    private void Update() {
        maxHealth = characterData.health.Value;
        if (health > maxHealth)
            health =  maxHealth;
    }

    public void ChangeHealth(float amountToChange){
        if (!isActive)
            return;

        health += amountToChange;
        health = Math.Min(health, maxHealth);
        health = Math.Max(0, health);

        if (health <= 0)
        {
            onHealthZero.Invoke();
        }
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
