using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
	public BulletType _myType;

	private GameObject gc;
    private GameObject bulletMngr;

    public float bulletDamage = 25f;

	//[SerializeField]
	//private LayerMask m_LayerMask;

	private float disappearTimer = 0f;
	[Tooltip("The amount of time for the current bullet to exist until it disappears")]
	public float timeToExist = 5f;

    private void Awake()
    {
		gc = GameObject.Find("GameController");
        bulletMngr = GameObject.Find("Bullet Manager");
    }

    private void Update()
    {
		OnBulletExist();
    }

    // Is used to make bullets disappear after certain amount of time, decluttering the cache
	private void OnBulletExist()
	{
        disappearTimer += Time.deltaTime;
        if (disappearTimer >= timeToExist)
        {
            bulletMngr.GetComponent<BulletManager>().AddBulletToCache(this);
            disappearTimer = 0f;
        }
    }

    /*    private void OnCollisionEnter(Collision collision)
        {
            //if ((int)m_LayerMask == ((int)m_LayerMask | (1 << collision.collider.gameObject.layer)))
            // If bullet touches environment stuffs ==> Make it disappear
            if (collision.collider.gameObject.layer == LayerMask.GetMask("Structures") || collision.collider.gameObject.CompareTag("Trees"))
            {
                bulletMngr.GetComponent<BulletManager>().AddBulletToCache(this);
                disappearTimer = 0f;
            }

            // IF bullet touches PLAYER ==> Damage the player + Make bullet disappear
            // We do need to add Damage to the bullet though (we can use a float or have TakeBulletFromCache pass the enemy as a param and then we get the CharacterData or sth idk)
            // Float is better since we don't have to think, plus Bullet dmg is at 25 or 40 in Vampire Survivors
            // See TouchDealDamage.cs and Damagable.cs --> DealDamage
            if (collision.gameObject.TryGetComponent(out Damageable damageObject) && collision.gameObject.tag == "Player")
            {
                //damageObject.DealDamage(-characterData.attackDamage.Value);
                damageObject.DealDamage(-bulletDamage);
                bulletMngr.GetComponent<BulletManager>().AddBulletToCache(this);
                disappearTimer = 0f;
            }
        }*/

    private void OnTriggerEnter(Collider collider)
    {
        //Debug.LogWarning(collider.name + " Hit! - Parent name: " + collider.GetComponentInParent<Transform>().gameObject.name + " - Parent mask: " + LayerMask.LayerToName(collider.GetComponentInParent<Transform>().gameObject.layer) );

        //if ((int)m_LayerMask == ((int)m_LayerMask | (1 << collision.collider.gameObject.layer)))
        // If bullet touches environment stuffs (Mainly TREES and STRUCTURES, small FOLIAGE liek Bushes don't count) ==> Make it disappear
        if (collider.gameObject.layer == LayerMask.NameToLayer("Structures") || collider.gameObject.CompareTag("Trees") || collider.CompareTag("Structures"))
        {
            bulletMngr.GetComponent<BulletManager>().AddBulletToCache(this);
            disappearTimer = 0f;
        }

        // IF bullet touches PLAYER ==> Damage the player + Make bullet disappear
        // We do need to add Damage to the bullet though (we can use a float or have TakeBulletFromCache pass the enemy as a param and then we get the CharacterData or sth idk)
        // Float is better since we don't have to think, plus Bullet dmg is at 25 or 40 in Vampire Survivors
        // See TouchDealDamage.cs and Damagable.cs --> DealDamage
        if (collider.gameObject.TryGetComponent(out Damageable damageObject) && collider.gameObject.CompareTag("Player"))
        {
            //damageObject.DealDamage(-characterData.attackDamage.Value);
            damageObject.DealDamage(-bulletDamage);
            bulletMngr.GetComponent<BulletManager>().AddBulletToCache(this);
            disappearTimer = 0f;
        }
    }

    private void OnEnable()
    {
		disappearTimer = 0f;
    }

    private void OnDisable()
    {
        disappearTimer = 0;
    }
}
