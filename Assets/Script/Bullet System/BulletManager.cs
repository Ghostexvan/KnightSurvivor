using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
	public List<BulletList> bulletTypeList;

	//public List<GameObject> _playerBulletCache;

	///public List<GameObject> _enemyBulletCache;

	//public GameObject _playerBulletPrefab;

	///public GameObject _enemyBulletPrefab;

	//public GameObject _playerBulletSpawnPoint;

	///public GameObject _enemyBulletParentObject;

	public Dictionary<BulletType, List<GameObject>> _caches;

    public Dictionary<BulletType, GameObject> _bulletPrefabs;

    public Dictionary<BulletType, GameObject> _bulletParents;

	[Header("Number of Pre Instantiated Bullets (UNUSED)")]
	public int numOfPreIns = 200;

	private void Start()
	{
/*		//_playerBulletCache = new List<GameObject>();
		_enemyBulletCache = new List<GameObject>();
		_bulletParents = new Dictionary<BulletType, GameObject>();
		//_bulletSpawnPoints.Add(BulletType.Player, _playerBulletSpawnPoint);
		_bulletParents.Add(BulletType.Enemy, _enemyBulletParentObject);
		for (int i = 0; i < numOfPreIns; i++)
		{
			//GameObject gameObject = Object.Instantiate(_playerBulletPrefab, _playerBulletSpawnPoint.transform.position, Quaternion.identity);
			//gameObject.SetActive(value: false);
			GameObject gameObject = Object.Instantiate(_enemyBulletPrefab, _enemyBulletParentObject.transform.position, Quaternion.identity);
			gameObject.SetActive(value: false);
			//gameObject.transform.parent = _bulletSpawnPoints[BulletType.Player].transform;
			gameObject.transform.parent = _bulletParents[BulletType.Enemy].transform;
			//_playerBulletCache.Add(gameObject);
			_enemyBulletCache.Add(gameObject);
		}
		_caches = new Dictionary<BulletType, List<GameObject>>();
		//_caches.Add(BulletType.Player, _playerBulletCache);
		_caches.Add(BulletType.Enemy, _enemyBulletCache);
		_bulletPrefabs = new Dictionary<BulletType, GameObject>();
		//_bulletPrefabs.Add(BulletType.Player, _playerBulletPrefab);
		_bulletPrefabs.Add(BulletType.Enemy, _enemyBulletPrefab);*/

		////////////////////////////////////// remove / cmt out the above
		if (bulletTypeList.Count > 0)
		{
			///==== Generic containers; mostly Dictionaries for: ====
			// BulletType and Parent Obj (to contain them PRE INSTANTIALIZED bullet objects wahoooooooo)
            _bulletParents = new Dictionary<BulletType, GameObject>();
			// BulletType and Corresponding List (containing that Bullet Type) - for us to track the cached bullet list
            _caches = new Dictionary<BulletType, List<GameObject>>();
            // BulletType and Prefab
            _bulletPrefabs = new Dictionary<BulletType, GameObject>();
            // All types of bullets must have different BulletType (enum) for this to work as is

			// Now we loop through each type of bullets to add into our Dicts
			for (int i = 0; i < bulletTypeList.Count; i++)
			{
				// Add BulletType - ParentObj
				_bulletParents.Add(bulletTypeList[i].bulletType, bulletTypeList[i]._enemyBulletParentObject);

				// Now we add them PRE INSTANTIALIZED bullets into the parent object
				for (int j = 0; j < bulletTypeList[i].numOfPreIns; j++)
				{
					// Pos and Rota is put there for the sake of it (lol)
					GameObject boulet = Instantiate(bulletTypeList[i]._enemyBulletPrefab, bulletTypeList[i]._enemyBulletParentObject.transform.position, Quaternion.identity);
					boulet.SetActive(false);
					boulet.transform.parent = _bulletParents[bulletTypeList[i].bulletType].transform;    // or we can go for bList[i]._enemyBulletParentObject.transform, either is fine tbh
					bulletTypeList[i]._enemyBulletCache.Add(boulet);

					// Add AddBulletToCache action in onHealthZero event
					boulet.gameObject.GetComponent<Health>().onBulletHealthZero.AddListener(AddBulletToCache);	// Parameter will be passed in Health.cs, rest assured it'll also be of type Bullet (and the current Bullet obj too, at that)
					//boulet.gameObject.GetComponent<Health>().onBulletHealthZero.AddListener(delegate { AddBulletToCache(boulet.GetComponent<Bullet>()); }) ;	// Parameter will be passed in Health.cs, rest assured it'll also be of type Bullet (and the current Bullet obj too, at that)
                }

				// Add BulletType - Corresponding List
				_caches.Add(bulletTypeList[i].bulletType, bulletTypeList[i]._enemyBulletCache);

				// Add BulletType - Prefab
				_bulletPrefabs.Add(bulletTypeList[i].bulletType, bulletTypeList[i]._enemyBulletPrefab);
				// This one is used in TakeBulletFromCache
            }
        }
	}

	public void AddBulletToCache(Bullet bullet)
	{
		bullet.gameObject.SetActive(value: false);
		_caches[bullet._myType].Add(bullet.gameObject);
	}

	public void TakeBulletFromCache(BulletType type, Vector3 spawnPos, Vector3 velocity)
	{
		List<GameObject> list = _caches[type];
		if (list.Count == 0)
		{
			Rigidbody component = Object.Instantiate(_bulletPrefabs[type], spawnPos, Quaternion.identity).GetComponent<Rigidbody>();
			component.transform.parent = _bulletParents[type].transform;
			component.velocity = velocity;
		}
		else
		{
			// Take bullet cache from the end of bullet list
			Rigidbody component2 = list[list.Count - 1].GetComponent<Rigidbody>();
			list.RemoveAt(list.Count - 1);
			component2.gameObject.SetActive(value: true);
			// Add bullet damage based on enemy damage here, or set it to straight up 25

			//// Add AddBulletToCache action in onHealthZero event
			//component2.gameObject.GetComponent<Health>().onBulletHealthZero.AddListener(AddBulletToCache);

			component2.transform.position = spawnPos;
			component2.velocity = velocity;
		}
	}


}

// I somehow forgot to add Serializable (lol) so it didn't appear in the Inspector, should be good now I think.
[System.Serializable]
public class BulletList
{
    public List<GameObject> _enemyBulletCache;
    public GameObject _enemyBulletPrefab;
    public GameObject _enemyBulletParentObject;
	public BulletType bulletType;

    [Header("Number of Pre Instantiated Bullets")]
    public int numOfPreIns = 200;

	public BulletList()
	{
		_enemyBulletCache = new List<GameObject>();
		numOfPreIns = 200;
	}
}

/**
	Make an SO or a class called BulletList, like so
	Then in BulletManager.cs (this current one), add 
	public List<BulletList> bl;
	
	Add the desired bullets and stuffs in the public BulletList bl
	In Start
	
		
		_enemyBulletCache = new List<GameObject>();
		_bulletSpawnPoints = new Dictionary<BulletType, GameObject>();
		//_bulletSpawnPoints.Add(BulletType.Player, _playerBulletSpawnPoint);
		_bulletSpawnPoints.Add(BulletType.Enemy, _enemyBulletParentObject);
		for (int i = 0; i < numOfPreIns; i++)
		{
			//GameObject gameObject = Object.Instantiate(_playerBulletPrefab, _playerBulletSpawnPoint.transform.position, Quaternion.identity);
			//gameObject.SetActive(value: false);
			GameObject gameObject = Object.Instantiate(_enemyBulletPrefab, _enemyBulletParentObject.transform.position, Quaternion.identity);
			gameObject.SetActive(value: false);
			//gameObject.transform.parent = _bulletSpawnPoints[BulletType.Player].transform;
			gameObject.transform.parent = _bulletSpawnPoints[BulletType.Enemy].transform;
			//_playerBulletCache.Add(gameObject);
			_enemyBulletCache.Add(gameObject);
		}
		_caches = new Dictionary<BulletType, List<GameObject>>();
		//_caches.Add(BulletType.Player, _playerBulletCache);
		_caches.Add(BulletType.Enemy, _enemyBulletCache);
		_bulletPrefabs = new Dictionary<BulletType, GameObject>();
		//_bulletPrefabs.Add(BulletType.Player, _playerBulletPrefab);
		_bulletPrefabs.Add(BulletType.Enemy, _enemyBulletPrefab);
 */
