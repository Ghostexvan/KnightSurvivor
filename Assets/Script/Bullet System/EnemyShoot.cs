using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyShoot : MonoBehaviour
{
	private Vector3 worldPos;

	private bool m_CanFireShotgun;

	private float m_CoolDownLeftShotgun;

	private bool m_TimerStartedShotgun;

	private float m_ShotgunAngleInterval;

	private GameObject player;

	private WaitForSeconds m_BetweenBurstsWait;

	private WaitForSeconds m_BetweenBurstShotsWait;

	private Coroutine currentCoroutine;

	[SerializeField]
	private float IntervalBetweenBursts = 2f;

	[SerializeField]
	private int ShotsInBurst = 3;

	[SerializeField]
	private float IntervalBetweenBurstShots = 0.2f;

	[SerializeField]
	private float RotationBetweenBurstShots = 10f;

	[SerializeField]
	private EnemySpreadStats m_SpreadStats;

	[SerializeField]
	private GameObject m_Bullet;

	private GameObject gc;
	private GameObject bulletMngr;

	// Min - max distance to shoot
	public float maxAttackDist;
	public float minAttackDist;

	// Bool to check whether Coroutine has started
	private bool hasCorouStarted;

	[Tooltip("Offset from player (mostly used for flower-y bullet patterns)")]
	public float targetPosOffset = 0f;

	protected void Start()
	{
		m_BetweenBurstsWait = new WaitForSeconds(IntervalBetweenBursts);
		m_BetweenBurstShotsWait = new WaitForSeconds(IntervalBetweenBurstShots);
		player = GameObject.Find("Player");
		gc = GameObject.Find("GameController");
		bulletMngr = GameObject.Find("Bullet Manager");

		//currentCoroutine = StartCoroutine("FireCycle");
		hasCorouStarted = false;
	}

	protected virtual IEnumerator FireCycle()
	{
		Debug.LogWarning("Corou is called");
		while (true)
		{
			yield return m_BetweenBurstsWait;
			// Get player's position
			Vector3 targetPos = player.transform.position;
			// Account player's height, have the bullet spawn at half the player's height (since the initial y value is technically 0)
			targetPos.y = player.GetComponent<BoxCollider>().bounds.extents.y;
			// Adding target offset if there's any, again this is only for flower-y bullet patterns
			targetPos.x += targetPosOffset;
			targetPos.z += targetPosOffset;

			for (int i = 0; i < ShotsInBurst; i++)
			{
				Shoot(targetPos, RotationBetweenBurstShots * (float)i);
				yield return m_BetweenBurstShotsWait;
			}
		}
	}

	private void Update()
	{
		ShootingControl();
	}

	// This is used to check whether Player is within enemy attack distance
	// If yes then Start FireCycle Coroutine and update hasCorouStarted bool value (to stop Coroutine from being triggered EVERY FRAME resulting in disasterous results)
	// If not then Stop FireCycle Coroutine and update hasCorouStarted bool value
	public void ShootingControl()
	{
        float distToPlayer = Vector3.Distance(this.transform.position, player.transform.position);

        if (distToPlayer <= maxAttackDist && distToPlayer >= minAttackDist)
        {
			//Debug.LogWarning("--- Player within attack distance");
            if (hasCorouStarted == false)
            {
                StartCoroutine(FireCycle());
                hasCorouStarted = true;
            }
        }
        else
        {
			if (hasCorouStarted == true)
			{
				StopShoot();
				//StopCoroutine(FireCycle());
				hasCorouStarted = false;
			}
        }
    }

	private void Shoot(Vector3 target, float offset)
	{
		m_ShotgunAngleInterval = m_SpreadStats.ShotgunSpread / m_SpreadStats.NumShotgunBullets * 2f;
		// Get target's (player) direction
		Vector3 vector = target - base.transform.position;
		vector.y = 0f;

        // player.GetComponent<Renderer>().bounds.extents.y
        Vector3 spawnPos = new Vector3(base.transform.position.x, target.y + 0.5f, base.transform.position.z);
		for (int i = 0; (float)i < m_SpreadStats.NumShotgunBullets; i++)
		{
			Vector3 vector2 = Quaternion.AngleAxis(0f - m_SpreadStats.ShotgunSpread + m_ShotgunAngleInterval * (float)i + m_ShotgunAngleInterval / 2f + offset, Vector3.up) * vector;
			// This Takes bullets from BulletType Enemy cache, I will add more later
			bulletMngr.GetComponent<BulletManager>().TakeBulletFromCache(BulletType.Enemy, spawnPos, vector2.normalized * m_SpreadStats.BulletSpeed);
		}
	}

	public void StopShoot()
	{
		StopAllCoroutines();
        Debug.LogWarning("Corou is stopped");
    }
}
