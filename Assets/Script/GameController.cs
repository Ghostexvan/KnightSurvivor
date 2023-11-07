using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public Vector3 planeCenter,
                   planeExtends;
    public Camera mainCamera;
    public bool isSet;
    public List<GameObject> enemyList;
    public int maxNumberEnemy;
    public GameObject enemyPrefab,
                      player,
                      enemyParent,
                      gameSettings,
                      gameOverPanel;
    public bool isBossBattle,
                isPlayerDeath;
    public float spawnRadius;

    //public List<SpawnWave> waves;

    private void Awake() {
        //Screen.SetResolution(1600, 900, true);
        Terrain _Data = transform.GetChild(0).gameObject.GetComponent<Terrain>();
        planeCenter = _Data.terrainData.bounds.center;
        planeExtends = _Data.terrainData.bounds.extents;
        //if (transform.GetChild(0).gameObject.GetComponent<Renderer>() == null)
        //{
        //    print("---Abayo");
        //}
        //else
        //{
        //    print("---Abayo2");

        //}
        //planeCenter = gameObject.transform.GetChild(0).gameObject.GetComponent<Renderer>().bounds.center;
        //planeExtends = gameObject.transform.GetChild(0).gameObject.GetComponent<Renderer>().bounds.extents;

        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        player = GameObject.FindGameObjectWithTag("Player");
        enemyParent = GameObject.Find("Enemy Pool");
        gameOverPanel.SetActive(false);
        isPlayerDeath = false;
        isBossBattle = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlayerDeath)
            return;

        if (player.GetComponent<Health>().health <= 0){
            gameOverPanel.SetActive(true);
            isPlayerDeath = true;
            return;
        }
        
        CheckEnemyList();
        if (isSet)
            StartCoroutine(SpawnEnemy());
    }

    IEnumerator SpawnEnemy(){
        while (enemyList.Count < maxNumberEnemy){
            //GameObject enemy = Instantiate(enemyPrefab, GetSpawnPosition(), Quaternion.identity, gameObject.transform.GetChild(1));
            // Fixed parent object for enemies
            GameObject enemy = Instantiate(enemyPrefab, GetSpawnPosition(), Quaternion.identity, enemyParent.transform);
            if (enemy)
            {
                print("---- DOES SPAWN ----");
            }
            enemyList.Add(enemy);
        }

        isSet = false;
        yield return null;
    }

    void CheckEnemyList(){   
        if (isSet)
            return;

        Debug.Log("Start check enemy list");
        if (enemyList.Count < maxNumberEnemy)
            isSet = true;

        foreach(GameObject enemy in enemyList){
            if (enemy == null){
                enemyList.Remove(enemy);
                isSet = true;
            }
        }
        Debug.Log("Done check enemy list");
    }

    Vector3 GetSpawnPosition(){
        Vector3 playerPosition = GetPlayerPosition();
        Vector3 position = new Vector3(playerPosition.x + UnityEngine.Random.Range(-spawnRadius, spawnRadius),
                                       5f,
                                       playerPosition.z + UnityEngine.Random.Range(-spawnRadius, spawnRadius));
        Debug.Log("Is in viewport: " + IsPositionOnCameraViewPort(position) + ", " + mainCamera.WorldToViewportPoint(position));
        while (IsPositionOnCameraViewPort(position) || !IsPositionInPlane(position)){
            position = new Vector3(playerPosition.x + UnityEngine.Random.Range(-spawnRadius, spawnRadius),
                                   5f,
                                   playerPosition.z + UnityEngine.Random.Range(-spawnRadius, spawnRadius));
        }

        return position;
    }

    bool IsPositionOnCameraViewPort(Vector3 position){
        if (mainCamera.WorldToViewportPoint(position).x >= 0 && mainCamera.WorldToViewportPoint(position).x <= 1 &&
            mainCamera.WorldToViewportPoint(position).y >= 0 && mainCamera.WorldToViewportPoint(position).y <= 1 &&
            mainCamera.WorldToViewportPoint(position).z >= 0)
            return true;
        return false;
    }

    bool IsPositionInPlane(Vector3 position){
        if (position.x < planeCenter.x - planeExtends.x || position.x > planeCenter.x + planeExtends.x)
            return false;

        if (position.z < planeCenter.z - planeExtends.z || position.z > planeCenter.z + planeExtends.z)
            return false;

        return true;
    }

    Vector3 GetPlayerPosition(){
        return player.transform.position;
    }
}

//[System.Serializable]
//public class SpawnWave
//{
//    public string waveName;
//    public List<GameObject> enemyPrefabs;
//    public List<string> enemyName;
//    public List<int> waveMaxEnemy;      // The max number of enemies allowed in this wave
//    public int waveQuota;               // The minimun number of enemies to spawn in this wave
//    public float spawnInterval;         // The interval between each enemy spawns
//    public int currentEnemyCount;       // The current number of enemies in this wave
//}

//public class EnemyGroup
//{

//}