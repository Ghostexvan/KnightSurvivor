using System;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public CharacterData characterData;
    public float health,
                 maxHealth;
    public UnityEvent onHealthZero;
    
    [Tooltip("Won't have anything as of NOW. But will have like freeze or explosion effects later on???")]
    public UnityEvent<Bullet> onBulletHealthZero;

    [Tooltip("Needed to be turned on in order for everything to work (more specifically ChangeHealth func)")]
    public bool isActive;

    [Header("Health Multipler")]
    [Tooltip("This is mostly used for Bosses with HP x Level Scaling.\n" +
        "This spans from 0.1 to 1, with 1 being the normal/base HP (100%).\n" +
        "If this is decreased then the total HP would be HPMultiplier * (HP x Level), which in turn would make the Bosses easier to beat.")]
    [Range(0.1f, 1f)]
    public float healthMultiplier = 1f;

    private GameObject player;

    /// <summary>
    /// REALLY, REALLY SCUFFED FIX, but it'll do for now (IT DIDN'T WORKKKKKKKKKK)
    /// ORIGINAL IDEA WORKED SO THIS IS REMOVED
    /// </summary>
    //private GameObject bulletMngr;


    private void Awake() {
        player = GameObject.Find("Player");

        // Init HP
        if (characterData != null)
        {
            maxHealth = characterData.health.Value;
            health = maxHealth;
        }
        //bulletMngr = GameObject.Find("Bullet Manager");
    }

    private void Start()
    {
        // HP Scaling is moved here since you can't really access parameters and/or properties of other Components in Awake
        // In Start, Player Obj and its Components (and the Comps' variables) will have existed
        if (characterData != null)
        {
            // Add another check for Bosses spawn
            // The conds will be if gameObject is not Player and characterData.isHPScale is true
            if (!this.gameObject.CompareTag("Player") && characterData.isHPScale == true)
            {
                // maxHP init will be: multiplier * (HP * player level)
                maxHealth = healthMultiplier * (characterData.health.Value * player.GetComponent<CharacterLevel>().currentLevel);
                health = maxHealth;
                // maxHP scaling is applied THE MOMENT THE ENEMY WAS SPAWNED, and WILL NOT BE UPDATED when Player gains levels when the current enemy is alive
            }
        }

    }

    private void Update() {
        // Is only used for Player since only Player can have their max HP changed at any given time/frame
        if (this.gameObject.name == "Player" && this.gameObject.CompareTag("Player"))
        {
            maxHealth = characterData.health.Value;
        }
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

            // Added checking script for Bullets (I love boulets)
            //if (this.gameObject.TryGetComponent<Bullet>(out Bullet bullet) && this.gameObject.CompareTag("Bullet"))
            //{
            //    onBulletHealthZero.Invoke(bullet);
            //}
            if (this.gameObject.CompareTag("Bullet"))
            {
                Debug.LogWarning("Bullet HP Zero");
                onBulletHealthZero.Invoke(this.gameObject.GetComponent<Bullet>());
                //bulletMngr.GetComponent<BulletManager>().AddBulletToCache(this.gameObject.GetComponent<Bullet>());
            }
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
