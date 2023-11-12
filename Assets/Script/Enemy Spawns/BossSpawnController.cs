using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(EnemySpawnController))]
public class BossSpawnController : MonoBehaviour
{
    public Vector3 planeCenter,
           planeExtends;
    public Camera mainCamera;
    public GameObject player,
                      bossParent;
    public float spawnRadius;
    public float teleportRadius;

    public EnemySpawnController esc;

    public List<BossWave> waves;

    // Wave manager variables
    [Header("Wave manager attributes")]
    [Tooltip("Current Boss Wave index. You don't need to edit this as this will be incremented automatically.\n" +
        "This will be grabbed from currentWaveIdx in EnemySpawnController.cs.")]
    public int currentBossWaveIdx;

    [Header("Spawner Attributes")]
    [Tooltip("Counts the current number of bosses on field - You don't need to edit this")]
    public int currentBossesAlive;
    [Tooltip("Max bosses allowed on stage at once")]
    public int maxBossesAllowed = 100;
    [Tooltip("Flag indicating if the maximum number of bosses has been reached - You don't need to edit this")]
    public bool maxBossesReached = false;
    [Tooltip("Flag to check whether wave is still active (Is used to fix the BeginNextWave problem) - You don't need to edit this")]
    bool isWaveActive = false;

    /// Used to despawn enemies
    //private List<GameObject> enemyList = new List<GameObject>();
    public List<GameObject> bossList = new List<GameObject>();

    public List<GameObject> crsh;
    [Tooltip("Number of enemies to check per frame")]
    public int bossesCheckPerFrame = 10;
    private int bossIdxToCheck = 0;    // Current enemy index to be checked in the list, default value is 0

    /// Used for despawning ALL enemies 
    [Tooltip("If this is enabled, the game will remove all enemies currently on the field WHEN entering the final ENEMY WAVE.\n" +
        "Bosses will be removed WHEN entering the final ENEMY WAVE. No bosses will be spawned after that.\n" +
        "The value of this depends on enableDespawnAll in EnemySpawnControl.cs.")]
    [HideInInspector]
    public bool enableDespawnAll = false;
    /// Flag to check whether DespawnAllEnemies has been called, this started off as False.
    /// We only wanted DespawnAllEnemies to run ONCE, then SpawnEnemies and DespawnEnemies will run as normal.
    [Tooltip("You needn't edit this.")]
    public bool resetAllEnemies = false;
    private int bossIdxToCheck_ = 0;   // Current enemy index to be checked in the list, default value is 0
    // This is used in DespawnAllEnemies

    /// <summary>
    /// Boss spawning variables
    /// </summary>
    private int oldBossWave;    // Used to check whether the current Wave has been changed
    private bool isBossSpawned; // Flag to check SpawnBoss() has been called ONCE.

    private void Awake()
    {
        Terrain _Data = transform.GetChild(0).gameObject.GetComponent<Terrain>();
        planeCenter = _Data.terrainData.bounds.center;
        planeExtends = _Data.terrainData.bounds.extents;

        //mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        player = GameObject.FindGameObjectWithTag("Player");
        bossParent = GameObject.Find("Boss Pool");


    }

    // Start is called before the first frame update
    void Start()
    {
        //bossList = new List<GameObject>();
        crsh = new List<GameObject>();

        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        // Get ESC Component
        esc = GameObject.Find("GameController").GetComponent<EnemySpawnController>();
        currentBossWaveIdx = esc.currentWaveIdx;
        enableDespawnAll = esc.enableDespawnAll;

        oldBossWave = -1;
        isBossSpawned = false;

        resetAllEnemies = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Liên tục lấy gtri curWaveIdx (vì ta cho BossSpawnController hoạt động theo việc dò wave
        // thay vì theo tgian)
        currentBossWaveIdx = esc.currentWaveIdx;

        // Nếu wave thay đổi, flag isBossSpawned đc reset
        if (oldBossWave != currentBossWaveIdx)
        {
            Debug.LogWarning("OldBossWave: " + oldBossWave);
            isBossSpawned = false;
        }

        // Liên tục lấy gtri isWaveActive (vì ta cho BossSpawnController hoạt động theo việc dò wave
        // thay vì theo tgian)
        isWaveActive = esc.isWaveActive;

        /// DO MAKE SURE THAT the numbers of Waves in both Enemy and Boss spawners are EQUAL
        if (enableDespawnAll)
        {
            /// Adding isBossSpawned = false to ensure SpawnBoss and DespawnAllBosses doesn't happen all at once in a single frame
            if (currentBossWaveIdx == esc.waves.Count - 1 && isWaveActive && resetAllEnemies == false && isBossSpawned == false)
            {
                // This is the same as DespawnEnemies, but without the Despawn Zone check
                // This is called once in every frame (since it's in Update) as long as the conds are met.
                // With each frame, this will delete a set number of enemies up to enemiesToCheck.
                // It will truly stop when enemyList.Count == 0.
                DespawnAllBosses();
                // resetAllEnemies will be set to true when enemyList.Count == 0

                // Giữ isBossSpawned == false để frame sau spawn boss thay vì SpawnBoss và Despawn ở cùng 1 frame
            }
        }
        /// Theo flow DespawnAll trc --> SpawnBoss

        // If wave is currently active + isBossSpawned == false, change oldBossWave's value and spawn boss(es) ONCE
        if (isWaveActive)
        {
            // This will be used to see if current wave has been changed or not in the upcoming frames
            // oldBossWave will be updated when current new wave is active
            oldBossWave = currentBossWaveIdx;

            // Spawning boss
            if (isBossSpawned == false)
            {
                // Check if current wave is the last; If it is and all enemies have been reset, spawn boss
                /// THIS is used the prevent the workflow where SpawnBoss and DespawnAllBosses happened all at once  in a single frame,
                /// resulting in the last boss not being able to spawn.
                /// And since SpawnBoss is only called exactly 1 time each wave
                if (currentBossWaveIdx == esc.waves.Count - 1)
                {
                    if (resetAllEnemies == true)
                    {
                        
                        Debug.LogWarning("Current oldBossWave: " + oldBossWave);
                        //StartCoroutine(SpawnBoss());
                        SpawnBoss();
                        isBossSpawned = true;
                    }
                }
                // Otherwise, spawn boss like normal
                else
                {
                    Debug.LogWarning("Current oldBossWave: " + oldBossWave);
                    //StartCoroutine(SpawnBoss());
                    SpawnBoss();
                    isBossSpawned = true;
                }
            }
        }

        // Đc gọi liên tục mỗi frame trong Update
        StartCoroutine(TeleportBoss());
    }

    // When this is called, isBossSpawned will be set to TRUE. isBossSpawned is used to check whether SpawnBoss() has been called or not.
    //IEnumerator SpawnBoss()
    //{
    //    if (isBossSpawned)
    //    {
    //        yield break;
    //    }

    //    // If BossWave is Empty
    //    if (waves[currentBossWaveIdx].bossGroup.Count <= 0 || waves[currentBossWaveIdx].numberToSpawn == 0)
    //    {
    //        isBossSpawned = true;
    //        Debug.LogWarning("--- EMPTY BOSS WAVE ---");
    //        yield break;
    //    }

    //    if (!maxBossesReached)
    //    {
    //        Debug.LogWarning("--- BOSS SPAWN ---");

    //        //int randomMode = Random.Range(1, 3);        // Random Mode 1 or Mode 2
    //        int randomMode = 1;
    //        if (waves[currentBossWaveIdx].bossGroup.Count == 1)
    //            randomMode = 1;
    //        /// Mode 1: Spawn Each type of boss in the current BossWave's bossGroup List
    //        if (randomMode == 1)
    //        {
    //            Debug.LogWarning("--- Random Boss Spawn 1 ---");
    //            foreach (var bossType in waves[currentBossWaveIdx].bossGroup)
    //            {
    //                GameObject boss = Instantiate(bossType.bossPrefab, GetSpawnPosition(bossType.bossPrefab), Quaternion.identity, bossParent.transform);
    //                boss.tag = "Enemy";
    //                // Calls OnEnemyKilled when object (boss) is destroyed / when onDestroy event (Destroyable.cs) is Invoked
    //                boss.GetComponent<Destroyable>().onDestroy.AddListener(OnBossKilled);
    //                // Calls IncreaseKillCount when enemy is DEFEATED by the player (onHealthZero event (Health.cs), since onDestroy event is used in Despawning)
    //                boss.GetComponent<Health>().onHealthZero.AddListener(esc.IncreaseKillCount);
    //                // Add to bossList (for despawn check)
    //                bossList.Add(boss);
    //                crsh.Add(boss);

    //                isBossSpawned = true;
    //                currentBossesAlive++;

    //                if (currentBossesAlive >= maxBossesAllowed)
    //                {
    //                    maxBossesReached = true;
    //                    yield break;
    //                }
    //            }

    //            yield break;
    //        }    
    //        //
    //        /// Mode 2: Spawn random types of bosses from the bossGroup List, with the number of bosses being numberToSpawn
    //        //if (randomMode == 2)
    //        //{
    //        //    Debug.LogWarning("--- Random Boss Spawn 2 ---");
    //        //    int bossGroupCount = waves[currentBossWaveIdx].bossGroup.Count;
    //        //    for (int i = 0; i < waves[currentBossWaveIdx].numberToSpawn; i++)
    //        //    {

    //        //        BossGroup bossGroup_ = waves[currentBossWaveIdx].bossGroup[Random.Range(0, bossGroupCount)];
    //        //        GameObject boss = Instantiate(bossGroup_.bossPrefab, GetSpawnPosition(bossGroup_.bossPrefab), Quaternion.identity, bossParent.transform);
    //        //        boss.tag = "Enemy";
    //        //        // Calls OnEnemyKilled when object (boss) is destroyed / when onDestroy event (Destroyable.cs) is Invoked
    //        //        boss.GetComponent<Destroyable>().onDestroy.AddListener(OnBossKilled);
    //        //        // Calls IncreaseKillCount when enemy is DEFEATED by the player (onHealthZero event (Health.cs), since onDestroy event is used in Despawning)
    //        //        boss.GetComponent<Health>().onHealthZero.AddListener(esc.IncreaseKillCount);
    //        //        // Add to bossList (for despawn check)
    //        //        bossList.Add(boss);

    //        //        isBossSpawned = true;
    //        //        currentBossesAlive++;

    //        //        if (currentBossesAlive >= maxBossesAllowed)
    //        //        {
    //        //            maxBossesReached = true;
    //        //            yield break;
    //        //        }
    //        //    }

    //        //    yield break;
    //        //}
    //        //
    //    }

    //    else
    //    {
    //        isBossSpawned = true;
    //        Debug.LogWarning("--- MAXED OUT NUMBER OF BOSSES ---");
    //        yield break;
    //    }

    //    yield break;
    //}
    void SpawnBoss()
    {
        if (isBossSpawned)
        {
            return;
        }

        // If BossWave is Empty
        if (waves[currentBossWaveIdx].bossGroup.Count <= 0 || waves[currentBossWaveIdx].numberToSpawn == 0)
        {
            isBossSpawned = true;
            Debug.LogWarning("--- EMPTY BOSS WAVE ---");
            return;
        }

        if (!maxBossesReached)
        {
            Debug.LogWarning("--- BOSS SPAWN ---");

            //int randomMode = Random.Range(1, 3);        // Random Mode 1 or Mode 2
            int randomMode = 1;
            if (waves[currentBossWaveIdx].bossGroup.Count == 1)
                randomMode = 1;
            /// Mode 1: Spawn Each type of boss in the current BossWave's bossGroup List
            if (randomMode == 1)
            {
                Debug.LogWarning("--- Random Boss Spawn 1 ---");
                foreach (var bossType in waves[currentBossWaveIdx].bossGroup)
                {
                    GameObject boss = Instantiate(bossType.bossPrefab, GetSpawnPosition(bossType.bossPrefab), Quaternion.identity, bossParent.transform);
                    boss.tag = "Enemy";
                    // Calls OnEnemyKilled when object (boss) is destroyed / when onDestroy event (Destroyable.cs) is Invoked
                    boss.GetComponent<Destroyable>().onDestroy.AddListener(OnBossKilled);
                    // Calls IncreaseKillCount when enemy is DEFEATED by the player (onHealthZero event (Health.cs), since onDestroy event is used in Despawning)
                    boss.GetComponent<Health>().onHealthZero.AddListener(esc.IncreaseKillCount);
                    // Add to bossList (for despawn check)
                    bossList.Add(boss);
                    crsh.Add(boss);

                    isBossSpawned = true;
                    currentBossesAlive++;

                    if (currentBossesAlive >= maxBossesAllowed)
                    {
                        maxBossesReached = true;
                        return;
                    }
                }

                return;
            }
            //
            /// Mode 2: Spawn random types of bosses from the bossGroup List, with the number of bosses being numberToSpawn
            //if (randomMode == 2)
            //{
            //    Debug.LogWarning("--- Random Boss Spawn 2 ---");
            //    int bossGroupCount = waves[currentBossWaveIdx].bossGroup.Count;
            //    for (int i = 0; i < waves[currentBossWaveIdx].numberToSpawn; i++)
            //    {

            //        BossGroup bossGroup_ = waves[currentBossWaveIdx].bossGroup[Random.Range(0, bossGroupCount)];
            //        GameObject boss = Instantiate(bossGroup_.bossPrefab, GetSpawnPosition(bossGroup_.bossPrefab), Quaternion.identity, bossParent.transform);
            //        boss.tag = "Enemy";
            //        // Calls OnEnemyKilled when object (boss) is destroyed / when onDestroy event (Destroyable.cs) is Invoked
            //        boss.GetComponent<Destroyable>().onDestroy.AddListener(OnBossKilled);
            //        // Calls IncreaseKillCount when enemy is DEFEATED by the player (onHealthZero event (Health.cs), since onDestroy event is used in Despawning)
            //        boss.GetComponent<Health>().onHealthZero.AddListener(esc.IncreaseKillCount);
            //        // Add to bossList (for despawn check)
            //        bossList.Add(boss);

            //        isBossSpawned = true;
            //        currentBossesAlive++;

            //        if (currentBossesAlive >= maxBossesAllowed)
            //        {
            //            maxBossesReached = true;
            //            yield break;
            //        }
            //    }

            //    yield break;
            //}
            //
        }

        else
        {
            isBossSpawned = true;
            Debug.LogWarning("--- MAXED OUT NUMBER OF BOSSES ---");
            return;
        }
    }

    //
    IEnumerator TeleportBoss()
    {
        if (bossList.Count == 0)
        {
            yield break;
        }

        // Index cuối của số enemy để while loop check tới, nếu enemyIdxToCheck vượt thì ngưng while loop
        int checkTarget = bossIdxToCheck + bossesCheckPerFrame - 1;
        // Cái này sẽ ko đề cập tới việc checkTarget > enemyList.Count - 1

        while (bossIdxToCheck <= checkTarget)
        {
            // Nếu idx hiện tại vẫn còn trong ngưỡng của List
            if (bossIdxToCheck <= bossList.Count - 1)
            {
                // Nếu enemy obj hiện tại trong list ko phải là null
                if (bossList[bossIdxToCheck] != null)
                {
                    // Nếu enemy nằm NGOÀI despawn area
                    // Teleport enemy đang xét ra chỗ khác
                    // ==> Sau đó ta cho bossIdxToCheck++ để xét Boss tiếp theo trong List
                    Vector3 playerPos = GetPlayerPosition();
                    if (bossList[bossIdxToCheck].transform.position.x > (playerPos.x + teleportRadius) ||
                        bossList[bossIdxToCheck].transform.position.x < (playerPos.x - teleportRadius) ||
                        bossList[bossIdxToCheck].transform.position.z > (playerPos.z + teleportRadius) ||
                        bossList[bossIdxToCheck].transform.position.z < (playerPos.z - teleportRadius))
                    {
                        bossList[bossIdxToCheck].transform.position = GetSpawnPosition(bossList[bossIdxToCheck]);
                    }
                    
                    bossIdxToCheck++;
                    
                }
                // THợp enemy bị killed thay vì despawned
                // ==> Bỏ khỏi List, đôn idx ktra lên (vì RemoveAt tự động đôn idx của các phần từ trong List)
                // ==> Giữ nguyên giá trị của bossIdxCheck để check với phần tử đc đôn lên
                else
                {
                    bossList.RemoveAt(bossIdxToCheck);
                    checkTarget--;
                }
            }
            // Nếu đã tới cuối List => reset cả idx và checkTarget
            // ==> Lúc này while không còn hợp lệ --> ngưng, để cho lần gọi sau checkTarget đc tính lại
            // Đây là lý do ta ko đề cập tới việc checkTarget > enemyList.Count - 1. Vì tới cuối List hiện tại là ta đã reset nó trước khi nó vượt enemyList.count - 1
            else
            {
                bossIdxToCheck = 0;
                checkTarget = 0 - 1;
            }
        }

        yield break;
    }

    //
    void DespawnAllBosses()
    {
        if (bossList.Count == 0)
        {
            Debug.LogWarning("--- ALL BOSSES HAVE BEEN DESPAWNED ---");
            resetAllEnemies = true;
            return;
        }

        Debug.LogWarning("--- DESPAWN ALL ENEMIES ---");

        // Index cuối của số enemy để while loop check tới, nếu enemyIdxToCheck vượt thì ngưng while loop
        int checkTarget = bossIdxToCheck_ + bossesCheckPerFrame - 1;
        // Cái này sẽ ko đề cập tới việc checkTarget > enemyList.Count - 1

        while (bossIdxToCheck_ <= checkTarget)
        {
            // Nếu idx hiện tại vẫn còn trong ngưỡng của List
            if (bossIdxToCheck_ <= bossList.Count - 1)
            {
                // Nếu enemy obj hiện tại trong list ko phải là null
                if (bossList[bossIdxToCheck_] != null)
                {
                    // Về cơ bản, ta sẽ xóa liên tục tại enemyIdxToCheck_ = 0. Cho tới khi nào checkTarget giảm xuống còn -1.
                    // Khi ta removeAt thì idx của các phần tử trong List được tự động đôn lên, nên sau khi xóa thì phần tử tiếp theo (idx=1) sẽ được đôn lên làm 0
                    // và được xóa tới tới.
                    // Việc tới cuối List là ko thể tại ta ko increment enemyIdxToCheck.
                    // Sau khi checkTarget = -1 thì while kết thúc. Nếu enemyList.Count chưa == 0 thì DespawnAllEnemies được gọi ở frame tiếp theo.
                    bossList[bossIdxToCheck_].GetComponent<Destroyable>().DestroyObject();
                    bossList.RemoveAt(bossIdxToCheck_);
                    checkTarget--;

                    //enemyIdxToCheck++;
                }
                // THợp enemy bị killed thay vì despawned
                // ==> Bỏ khỏi List, đôn idx ktra lên (vì RemoveAt tự động đôn idx của các phần từ trong List)
                // ==> Giữ nguyên giá trị của bossIdxCheck để check với phần tử đc đôn lên
                else
                {
                    bossList.RemoveAt(bossIdxToCheck_);
                    checkTarget--;
                }
            }
            /// === Trường hợp này sẽ ko bao giờ xảy ra trong hàm này btw ===
            // Nếu đã tới cuối List => reset cả idx và checkTarget
            // ==> Lúc này while không còn hợp lệ --> ngưng, để cho lần gọi sau checkTarget đc tính lại
            // Đây là lý do ta ko đề cập tới việc checkTarget > enemyList.Count - 1. Vì tới cuối List hiện tại là ta đã reset nó trước khi nó vượt enemyList.count - 1
            else
            {
                bossIdxToCheck_ = 0;
                checkTarget = 0 - 1;
            }
        }
    }

    // Decrement enemy count when it is killed
    // This func will be called when an enemy is killed (and is put in onDestroy event (Destroyable.cs) since despawning also removes enemies alive)
    public void OnBossKilled()
    {
        currentBossesAlive--;

        /// 3. Resets the maxEnemies reached flag if the number of enemies alive has dropped below the max amount
        if (currentBossesAlive < maxBossesAllowed)
        {
            maxBossesReached = false;
        }
    }

    // Taken from GameController.cs
    Vector3 GetSpawnPosition(GameObject enemyPrefab)
    {
        Vector3 playerPosition = GetPlayerPosition();
        // Get Collider y extent
        float yExtent = enemyPrefab.GetComponent<BoxCollider>().bounds.extents.y;

        Vector3 position = new Vector3(playerPosition.x + UnityEngine.Random.Range(-spawnRadius, spawnRadius),
                                       yExtent + .1f,
                                       playerPosition.z + UnityEngine.Random.Range(-spawnRadius, spawnRadius));
        Debug.Log("Is in viewport: " + IsPositionOnCameraViewPort(position) + ", " + mainCamera.WorldToViewportPoint(position));
        // Lý do nhầm lúc IsPositionOnCameraViewport không hoạt động là vì position.y ban đầu = 5f, có thể nằm ngoài cam
        // Ta đổi y lại thành yExtent + .1f, để enemy có thể spawn mà ko bị clip xuống dưới
        while (IsPositionOnCameraViewPort(position) || !IsPositionInPlane(position) || IsPositionCollide(position, enemyPrefab))
        {
            position = new Vector3(playerPosition.x + UnityEngine.Random.Range(-spawnRadius, spawnRadius),
                                   yExtent + .1f,
                                   playerPosition.z + UnityEngine.Random.Range(-spawnRadius, spawnRadius));
        }

        return position;
    }

    bool IsPositionOnCameraViewPort(Vector3 position)
    {
        if (mainCamera.WorldToViewportPoint(position).x >= 0 && mainCamera.WorldToViewportPoint(position).x <= 1 &&
            mainCamera.WorldToViewportPoint(position).y >= 0 && mainCamera.WorldToViewportPoint(position).y <= 1 &&
            mainCamera.WorldToViewportPoint(position).z >= 0)
            return true;
        return false;
    }

    bool IsPositionInPlane(Vector3 position)
    {
        if (position.x < planeCenter.x - planeExtends.x || position.x > planeCenter.x + planeExtends.x)
            return false;

        if (position.z < planeCenter.z - planeExtends.z || position.z > planeCenter.z + planeExtends.z)
            return false;

        return true;
    }

    bool IsPositionCollide(Vector3 position, GameObject enemyPrefab)
    {
        float xExtents = enemyPrefab.GetComponent<BoxCollider>().bounds.extents.x;
        float yExtents = enemyPrefab.GetComponent<BoxCollider>().bounds.extents.y;
        float zExtents = enemyPrefab.GetComponent<BoxCollider>().bounds.extents.z;

        // Use OverlapBox to check if enemy's x and z extents actually collides with something, if it does then return true
        // (Yeah collides with something other than the bushes by the way, bushes are safe)
        Collider[] hitColliders = Physics.OverlapBox(new Vector3(position.x, 1.5f, position.y), new Vector3(xExtents, 0.5f, zExtents), Quaternion.identity);
        if (hitColliders.Length > 0)
        {
            foreach (Collider collider in hitColliders)
            {
                if (!collider.CompareTag("Foliage") || collider.gameObject.layer == LayerMask.NameToLayer("Structures") || collider.gameObject.layer == LayerMask.NameToLayer("Wall"))
                    return true;
            }
        }

        // else, return false --> Position has no collisions
        return false;
    }

    Vector3 GetPlayerPosition()
    {
        return player.transform.position;
    }

    /// <summary>
    /// FOR FUTURE (idk like a couple of weeks/next month) REFERENCE
    /// 
    /// THIS IS USED WHEN WE ADDED MODELS TO THE ENEMIES/BOSSES.
    /// THEY SHOULD FOLLOW THIS PREFAB TREE:
    /// Parent:
    ///     Model + Collider + Scripts
    /// PREFAB TREE WILL BE SIMILAR TO THE ONES IN Buildings Prefabs.
    /// 
    /// You only needed to change to yExtent in GetSpawnPosition, using this func to get the new yExtent
    /// </summary>
    /// <param name="go"> Parameter will be a GameObject that contains Renderers </param>
    /// <returns> The Bounds of an Object with multiple children meshes </returns>
    Bounds GetChildRendererBounds(GameObject go)
    {
        Renderer[] renderers = go.GetComponentsInChildren<Renderer>();

        if (renderers.Length > 0)
        {
            Bounds bounds = renderers[0].bounds;
            for (int i = 1, ni = renderers.Length; i < ni; i++)
            {
                bounds.Encapsulate(renderers[i].bounds);
            }
            return bounds;
        }
        else
        {
            return new Bounds();
        }
    }
}
