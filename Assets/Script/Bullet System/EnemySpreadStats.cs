using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/EnemySpreadStats", order = 1)]
public class EnemySpreadStats : ScriptableObject
{
	public float FireCooldown = 1f;

	public float BulletSpeed = 3f;

	public float NumShotgunBullets = 3f;

	public float ShotgunSpread = 1f;
}
