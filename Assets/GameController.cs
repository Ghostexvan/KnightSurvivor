using System.Collections.Generic;
using System.IO.Compression;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public Vector3 planeCenter,
                   planeExtends;
    public Camera mainCamera;
    public bool isSet;
    public List<GameObject> enemyList;
    public int maxNumberEnemy;
    public GameObject enemyPrefab;

    private void Awake() {
        planeCenter = gameObject.transform.GetChild(0).gameObject.GetComponent<Renderer>().bounds.center;
        planeExtends = gameObject.transform.GetChild(0).gameObject.GetComponent<Renderer>().bounds.extents;
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckEnemyList();
        if (isSet)
            SpawnEnemy();
    }

    void SpawnEnemy(){
        while (enemyList.Count < maxNumberEnemy){
            GameObject enemy = Instantiate(enemyPrefab, GetSpawnPosition(), Quaternion.identity, gameObject.transform.GetChild(1));
            enemyList.Add(enemy);
        }

        isSet = false;
    }

    void CheckEnemyList(){
        if (enemyList.Count < maxNumberEnemy)
            isSet = true;

        foreach(GameObject enemy in enemyList){
            if (enemy == null){
                enemyList.Remove(enemy);
                isSet = true;
            }
        }
    }

    Vector3 GetSpawnPosition(){
        Vector3 position = new Vector3(planeCenter.x + Random.Range(-planeExtends.x, planeExtends.x),
                                       5f,
                                       planeCenter.z + Random.Range(-planeExtends.z, planeExtends.z));
        Debug.Log("Is in viewport: " + IsPositionOnCameraViewPort(position) + ", " + mainCamera.WorldToViewportPoint(position));
        while (IsPositionOnCameraViewPort(position)){
            position = new Vector3(planeCenter.x + Random.Range(-planeExtends.x, planeExtends.x),
                                   planeCenter.y + Random.Range(-planeExtends.y, planeExtends.y));
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
}
