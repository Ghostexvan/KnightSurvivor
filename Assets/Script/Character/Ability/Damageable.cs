using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    public bool isActive;
    public UnityEvent<float> onDamaged;

    // Enemy damage feedback
    // Player and Potentially Modeled Enemies
    SkinnedMeshRenderer[] skinnedMeshList;
    MeshRenderer enemyMesh;
    [Header("Damage feedback variables")]
    // Contains default values, you can change them in the inspector
    public float blinkDuration = 0.05f;
    public float blinkIntensity = 10;
    float blinkTimer;   //
    
    /// YOU DON'T NEED TO EDIT BOTH OF THESE
    [Tooltip("Used to determine whether to use WHITE (for enemies) or RED (for player) flash")]
    public bool isPlayer;

    [Tooltip("Used to determine whether this is a BULLET or not, if it is then no need to display DamageText")]
    public bool isBullet;
    /// YOU DON'T NEED TO EDIT BOTH OF THESE

    // Is used to determine knockback direction
    private GameObject player;
    private Rigidbody currentRB;

    [Header("Data variables")]
    public CharacterData characterData;
    public GameObject damageText;
    public bool isCrit;

    private void Start()
    {
        // Find out whether the current Object is Player or not
        isPlayer = this.CompareTag("Player");
        if (isPlayer)
        {
            skinnedMeshList = GetComponentsInChildren<SkinnedMeshRenderer>();
        }
        else
        {
            player = GameObject.Find("Player");
            currentRB = GetComponent<Rigidbody>();

            enemyMesh = GetComponent<MeshRenderer>();
        }

        isBullet = this.CompareTag("Bullet");
        //blinkTimer = 0;
    }

    public void DealDamage(float damageAmount){
        if (!isActive)
            return;

        StopCoroutine(FlashFeedback());
        StartCoroutine(FlashFeedback());

        // If current Object is anything but a Bullet, instantiate damageText as usual
        if (!isBullet)
        {
            //Debug.Log(damageAmount + characterData.defense.GetFinalValue());
            GameObject damageTextInstance = Instantiate(damageText, 
                                                        new Vector3(gameObject.transform.position.x + Random.Range(-1f, 1f), 
                                                                    gameObject.transform.position.y + Random.Range(0f, 2f), 
                                                                    gameObject.transform.position.z + Random.Range(-1f, -0.1f) - GetComponent<Collider>().bounds.size.z/2
                                                        ),
                                                        Quaternion.identity,
                                                        gameObject.transform);
            //damageTextInstance.transform.localPosition = transform.TransformPoint(gameObject.transform.position);
            damageTextInstance.SendMessage("SetValue", System.Math.Max(-damageAmount - characterData.defense.Value, 0));
            Debug.Log("Expect text position: " + gameObject.transform.position);

            if (isCrit)
                damageTextInstance.SendMessage("SetCrit");
            onDamaged.Invoke(System.Math.Min(damageAmount + characterData.defense.Value, 0));
        }
        // If current Object is indeed a Bullet, inflict true damage, don't add damageText (Flash will still be added tho)
        else
        {
            // Call onDamaged event on Bullets
            onDamaged.Invoke(System.Math.Min(damageAmount, 0));
        }

        // onDamaged.Invoke(damageAmount);

        // Activate DamageFeedback func by changing the value of blinkTimer = blinkDuration
        //blinkTimer = blinkDuration;

        // Do the knockback as well
    }

    public void SetCrit(bool isCrit){
        this.isCrit = isCrit;
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

    // Enemy Damage Feedback
    private void Update()
    {
        // 
        //DamageFeedback();
        //if (blinkTimer >= 0)
        //{
        //}

        //if (blinkTimer == 0) 
        //    blinkTimer = 0;
    }

    /// <summary>
    /// FAILED ATTEMPTS ARE KEPT HERE
    /// (this is where I wrote out my ideas but they didn't go as planned, so I scrapped them and turn to another solution)
    /// P.S: I tend to overcomplicate things than they needed to be
    /// </summary>
    /// <returns></returns>
    // Changes color of damaged object
    //void DamageFeedback()
    //{
    //    blinkTimer -= Time.deltaTime;
    //    // Clamps value between 0 and 1 and returns value. 
    //    // If the value is negative then zero is returned.If value is greater than one then one is returned.
    //    float lerp = Mathf.Clamp01(blinkTimer / blinkDuration);
        
    //    // Bare minimum of intensity must at least be 1.0f in order for the Material to be displayed normally.
    //    float intensity = (blinkIntensity * lerp) + 1.0f;

    //    // Does damage feedback based on the current object
    //    if (isPlayer)
    //    {
    //        if (skinnedMeshList.Count<SkinnedMeshRenderer>() > 0)
    //        {
    //            foreach(SkinnedMeshRenderer skm in skinnedMeshList)
    //            {
    //                skm.material.color = Color.red * intensity;
    //            }
    //        }
    //    }
    //    else
    //    {
    //        enemyMesh.material.color = Color.white * intensity;
    //    }
    //}
    ///== FAILED ATTEMPT END ==

    IEnumerator FlashFeedback()
    {
        float emission = Mathf.PingPong(Time.time, 1.0f);
        float duration = blinkDuration;
        Color baseColor = Color.white;

        if (isPlayer)
            baseColor = Color.red;

        Color finalColor = baseColor * Mathf.LinearToGammaSpace(emission);

        /// FAILED ATTEMPT #2
        //while(duration > 0)
        //{
        //    //
        //    duration -= Time.deltaTime;
        //    if (isPlayer)
        //    {
        //        if (skinnedMeshList.Count<SkinnedMeshRenderer>() > 0)
        //        {
        //            foreach (SkinnedMeshRenderer skm in skinnedMeshList)
        //            {
        //                //skm.material.color = finalColor;
        //                skm.material.EnableKeyword("_EMISSION");
        //                skm.material.SetColor("_EmissionColor", finalColor);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        //enemyMesh.material.color = finalColor;
        //        enemyMesh.material.EnableKeyword("_EMISSION");
        //        enemyMesh.material.SetColor("_EmissionColor", finalColor);
        //    }


        //    //yield return null;
        //}
        /// FAILED ATTEMPT #2 (I was still sticking with the Time.deltaTime method I forgot WatiForSeconds exists)

        if (isPlayer)
        {
            if (skinnedMeshList.Count<SkinnedMeshRenderer>() > 0)
            {
                foreach (SkinnedMeshRenderer skm in skinnedMeshList)
                {
                    //skm.material.color = finalColor;
                    skm.material.EnableKeyword("_EMISSION");
                    //skm.material.SetColor("_EmissionColor", finalColor);
                    skm.material.SetColor("_EmissionColor", baseColor);
                }
            }
        }
        else
        {
            //enemyMesh.material.color = finalColor;
            enemyMesh.material.EnableKeyword("_EMISSION");
            //enemyMesh.material.SetColor("_EmissionColor", finalColor);
            enemyMesh.material.SetColor("_EmissionColor", baseColor);
        }

        yield return new WaitForSeconds(duration);

        if (isPlayer)
        {
            if (skinnedMeshList.Count<SkinnedMeshRenderer>() > 0)
            {
                foreach (SkinnedMeshRenderer skm in skinnedMeshList)
                {
                    //skm.material.color = finalColor;
                    skm.material.SetColor("_EmissionColor", Color.black);
                    skm.material.DisableKeyword("_EMISSION");
                }
            }
        }
        else
        {
            //enemyMesh.material.color = finalColor;
            enemyMesh.material.SetColor("_EmissionColor", Color.black);
            enemyMesh.material.DisableKeyword("_EMISSION");
        }

        StopCoroutine(FlashFeedback());
    }

    public void KnockbackFeedback(float damage)
    {
        Debug.LogWarning("Damage Received: " + damage);

        if (damage < 0) damage = -damage;
        float knockbackForce = damage / 10f;
        //Vector3 direction = (this.transform.position * -1f).normalized;

        if (!isPlayer)
        {
            // I don't get why this worked but it seemed to work. I'll look more into it later.
            //Vector3 direction = -(player.transform.position - this.transform.position).normalized;
            // Directional vector from Player pos to Monster pos; this also reflects the direction the Enemy was hit to
            // Basically it's a Vector from the Player to the Enemy, which is also the knockback direction
            Vector3 direction = (this.transform.position - player.transform.position).normalized;

            this.transform.Translate(direction * knockbackForce, Space.World);
            //this.transform.Translate(direction * knockbackForce);
            //currentRB.AddForce(currentRB.velocity * -1, ForceMode.Impulse);
        }
    }
}
