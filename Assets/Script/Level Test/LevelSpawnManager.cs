using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(TreeGenerator), typeof(StructureGenerator), typeof(FoliageGenerator))]
public class LevelSpawnManager : MonoBehaviour
{
    [Header("Turning this script off would basically mean turning the LEVEL GENERATION + ENEMY SPAWNING off.")]
    public bool generateStructures = true;

    private TreeGenerator tg;
    private StructureGenerator sg;
    private FoliageGenerator fg;
    private GenerateNavMesh gnvm;

    private GameObject structureBlockedArea;

    private GameObject player;

    private GameObject gameController;

    private GameObject gameCanvas;

    public bool isPaused = false;

    private void Awake()
    {
        tg = GetComponent<TreeGenerator>();
        sg = GetComponent<StructureGenerator>();
        fg = GetComponent<FoliageGenerator>();
        gnvm = GetComponent<GenerateNavMesh>();

        //player = GameObject.Find("Player");
        //player.SetActive(false);

        //structureBlockedArea = GameObject.Find("Struct Blocked Area");
        ////structureBlockedArea.GetComponent<MeshCollider>().isTrigger = true;
        //structureBlockedArea.SetActive(false);
        player = GameObject.Find("Player");
        gameController = GameObject.Find("GameController");
        gameCanvas = GameObject.Find("GameCanvas");
    }

    // Start is called before the first frame update
    void Start()
    {
        // Get refs (you can call in Awake but it's safer in Start since everything's been initialized at this point)

        // Modify stuffs in 
        // player.SetActive(false);

        // ORIGINALLY, I wanted to DISABLE the player before letting everything (Trees, Structures, Smaller Foliage) spawn. Then after that ENABLING the player
        // But I realized that the GameCanvas is pretty much dependant on the Player Object, so I went for another direction (and it worked, somehow)
        // By setting Time.timeScale = 0 and maxNumberEnemy = 0, I can prevent both:
        // 1. Enemy spawn killing the player
        // 2. UDP Controller receiving controls when our environment is being GENERATED
        // After everything, I changed the Time.timeScale = 1 and maxNumberEnemy = 100, allowing the game to run like normal (will it though?)

        Time.timeScale = 0f;
        isPaused = true;
        //gameController.SetActive(false);

        // Old spawning system
        //gameController.GetComponent<GameController>().maxNumberEnemy = 0;

        //gameController.GetComponent<EnemySpawnController>().waves[0].minWaveQuota = 0;
        //Debug.LogWarning("Map Gen Quota: " + gameController.GetComponent<EnemySpawnController>().waves[0].minWaveQuota);
        gameController.GetComponent<EnemySpawnController>().enabled = false;
        gameController.GetComponent<BossSpawnController>().enabled = false;
        gameController.GetComponent<PythonScriptCall>().enabled = false;

        structureBlockedArea = GameObject.Find("Struct Blocked Area");
        //structureBlockedArea.GetComponent<MeshCollider>().isTrigger = true;
        structureBlockedArea.SetActive(false);

        StartCoroutine(InitField());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator EnablePlayer()
    {
        player.SetActive(true);
        
        //Time.timeScale = 1f;

        yield return null;
    }

    IEnumerator EnableEnemySpawner()
    {
        yield return null;
    }

    IEnumerator InitField()
    {
        // Each Coroutine will wait for the prior one to finish, hence yield return StartCorou
        // Normally they'd be more on the asynchronous side
        yield return StartCoroutine(tg.SpawnTrees());
        if (generateStructures)
        {
            structureBlockedArea.SetActive(true);
            yield return StartCoroutine(sg.SpawnStructures());
        }
        // Happens right after Struct Generation is finished
        if (structureBlockedArea.activeSelf)
        {
            structureBlockedArea.SetActive(false);
        }
        yield return StartCoroutine(fg.SpawnFoliage());

        yield return StartCoroutine(gnvm.CreateNavmesh());
        //player.SetActive(true);
        //Time.timeScale = 1f;
        yield return StartCoroutine(EnablePlayer());
        // Phải enable player trước để UI và EnemySpawnController có thể lấy các giá trị của Player 

        gameController.GetComponent<PythonScriptCall>().enabled = true;
        // Old spawning system
        //gameController.GetComponent<GameController>().maxNumberEnemy = 10;

        //gameController.GetComponent<EnemySpawnController>().CalculateWaveQuota();
        Debug.LogWarning("Map Gen Quota 2: " + gameController.GetComponent<EnemySpawnController>().waves[0].minWaveQuota);
        // Cho spawnTimer = spawnInterval để có thể spawn liền 1 đợt enemy lúc mới khởi tạo map
        gameController.GetComponent<EnemySpawnController>().spawnTimer = gameController.GetComponent<EnemySpawnController>().waves[0].spawnInterval;
        // Cho Time.timeScale = 1f
        Time.timeScale = 1f;
        gameController.GetComponent<EnemySpawnController>().enabled = true;
        gameController.GetComponent<BossSpawnController>().enabled = true;

        /// --------------- Game timer starts here --------------- ///

        print("--- Finished ---");
    }
}
