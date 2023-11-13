using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnController : MonoBehaviour
{
    public Vector3 planeCenter,
               planeExtends;
    public Camera mainCamera;
    public GameObject player,
                      enemyParent;
    public float spawnRadius;
    public float despawnRadius;

    public List<SpawnWave> waves;

    // Wave manager variables
    [Header("Wave manager attributes")]
    [Tooltip("Current Wave index. You don't need to edit this as this will be incremented automatically.")]
    public int currentWaveIdx;
    //[Tooltip("Final Wave index. When currentWaveIdx reaches this point, it will stay in this wave until the game ends.\n" +
    //    "EX: Since indexes starts from 0, if you want your final wave to be at 30 then you should put this value as 30-1 = 29.")]
    //public int finalWaveIdx;    // Can be used to replace wave.Count (or wave.Count - 1 if we're talking about index)

    [Header("Spawner Attributes")]
    [Tooltip("Timer used to determine when to spawn the next enemy, basically a spawn interval/spawn rate - Is not visible in inspector btw")]
    [HideInInspector]
    public float spawnTimer;
    [Tooltip("The interval between each wave")]
    public float waveInterval;
    [Tooltip("Counts the current number of enemies on field - You don't need to edit this")]
    public int currentEnemiesAlive;
    [Tooltip("Max enemies allowed on stage at once")]
    public int maxEnemiesAllowed;
    [Tooltip("Flag indicating if the maximum number of enemies has been reached - You don't need to edit this")]
    public bool maxEnemiesReached = false;
    [Tooltip("Flag to check whether wave is still active (Is used to fix the BeginNextWave problem) - You don't need to edit this")]
    public bool isWaveActive = false;
    
    /// Used to despawn enemies
    //private List<GameObject> enemyList = new List<GameObject>();
    public List<GameObject> enemyList;
    [Tooltip("Number of enemies to check per frame")]
    public int enemiesCheckPerFrame;
    private int enemyIdxToCheck = 0;    // Current enemy index to be checked in the list, default value is 0

    /// Used for despawning ALL enemies 
    [Tooltip("If this is enabled, the game will remove all enemies currently on the field WHEN entering the final ENEMY WAVE.\n" +
        "Enemies will be removed WHEN entering the final ENEMY WAVE but before SpawnEnemies")]
    public bool enableDespawnAll = false;
    /// Flag to check whether DespawnAllEnemies has been called, this started off as False.
    /// We only wanted DespawnAllEnemies to run ONCE, then SpawnEnemies and DespawnEnemies will run as normal.
    [Tooltip("You needn't edit this.")]
    public bool resetAllEnemies = false;
    private int enemyIdxToCheck_ = 0;   // Current enemy index to be checked in the list, default value is 0
    // This is used in DespawnAllEnemies


    /// Used to count the total number of enemies killed
    public int totalEnemiesKilled = 0;


    /// This is the Winning condition flag.
    [Tooltip("You needn't edit this.")]
    public bool hasPlayerWon = false;


    private void Awake()
    {
        Terrain _Data = transform.GetChild(0).gameObject.GetComponent<Terrain>();
        planeCenter = _Data.terrainData.bounds.center;
        planeExtends = _Data.terrainData.bounds.extents;

        spawnTimer = 0;     // Init spawnTimer (default value of float is 0f already, this is written to make the logic flow look better)
        totalEnemiesKilled = 0;     // Init totalEnemiesKilled

        //mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        player = GameObject.FindGameObjectWithTag("Player");
        enemyParent = GameObject.Find("Enemy Pool");
    }

    // Start is called before the first frame update
    void Start()
    {
        enemyList = new List<GameObject>();

        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        currentWaveIdx = 0;

        resetAllEnemies = false;
        hasPlayerWon = false;
        //CalculateWaveQuota();
        //SpawnEnemies();
    }

    // Update is called once per frame
    void Update()
    {
        ExecuteSpawnController();
    }

    /// <summary>
    /// This is mainly to put everything from the Update into a single func for simplicity.
    /// This is also to check whether Player object does exist or not. 
    /// If the Player object is DELETED from the game scene (When Player dies, DestroyObject in Destroyable.cs deletes/destroys the Player GameObject)
    /// </summary>
    public void ExecuteSpawnController()
    {
        /// IF PLAYER OBJECT IS DELETED THEN STOP EXECUTING THIS FUNC
        if (player == null)
            return;
        /// IF NUMBER OF WAVES IS LITERALLY 0 THEN STOP EXECUTING THIS FUNC
        if (waves.Count == 0)
            return;

        /// NOTES (for meself)
        // Nếu wave hiện tại chưa là wave cuối cùng và wave hiện tại mới đc khởi tạo (spawnCount lúc này = 0)
        // ==> Gọi Coroutine BeginNextWave(), nó sẽ đợi cho hết tgian của wave hiện tại và gọi wave tiếp theo (asynchronously ofc)
        // Khi đó, điều kiện này sẽ được xét lại với wave tiếp theo (là wave cuối và mới đc khởi tạo). Tại thời điểm đó, wave tiếp theo SẼ là wave hiện tại
        // Và mọi thứ được lặp lại cho tới khi hết số wave
        // ------------------------------------------------------------------------
        // Nhưng nếu có 3 waves trở lên, nó sẽ gọi cái này LIÊN TỤC cho tới wave cuối cùng
        // Lý do rất đơn giản: điều kiện currentWaveIdx < waves.Count && waves[currentWaveIdx].currentSpawnCount == 0 được check MỖI FRAME (tại vì ta đang trong Update)
        // Mà Enemies thì ta spawn sau spawnInterval GIÂY ==> Đâm ra điều kiện được check mỗi frame và BeginNextWave được gọi dồn liên tục (trong vòng trước 1s), cho tới khi currentWaveIdx++ == waves.Count - 1
        // thì BeginNextWave nó ko cộng thêm đc nữa, còn các Coroutine thừa vẫn cứ đc execute (và ko cộng đc)
        /// Để xử lý vấn đề, ta thêm flag check BeginNextWave có đang chạy hay không (isWaveActive)
        if (currentWaveIdx < waves.Count && waves[currentWaveIdx].currentSpawnCount == 0 && !isWaveActive)
        {
            StartCoroutine(BeginNextWave());
        }

        /// The FINAL WAVE (idx == waves.Count - 1) usually consist of ONLY INSTA-killing enemies.
        /// But before that, all enemies currently on the field will be despawned.
        /// Will only be enabled if enableDespawnAll is true
        /// This will be called BEFORE SpawnWave runs, so it follows the the:
        /// DespawnAll --> Spawn Last Wave Enemies flow
        if (enableDespawnAll)
        {
            if (currentWaveIdx == waves.Count - 1 && isWaveActive && resetAllEnemies == false)
            {
                // This is the same as DespawnEnemies, but without the Despawn Zone check
                // This is called once in every frame (since it's in Update) as long as the conds are met.
                // With each frame, this will delete a set number of enemies up to enemiesToCheck.
                // It will truly stop when enemyList.Count == 0.
                DespawnAllEnemies();
                // resetAllEnemies will be set to true when enemyList.Count == 0
            }
        }

        //SpawnEnemies();
        spawnTimer += Time.deltaTime;

        /// We don't need to check whether DespawnAllEnemies and SpawnEnemies happen at the same time (like how the BossSpawner does) because:
        /// 1. It's pretty hard for spawnTimer to stop right at DespawnAllEnemies
        /// 2. Even if it does, enemies would spawn in hordes to fill the minimum wave quota regardless
        if (spawnTimer >= waves[currentWaveIdx].spawnInterval)
        {
            spawnTimer = 0;
            SpawnEnemies();
            Debug.LogWarning("SpawnEnemies called");
        }
        
        // Đc gọi liên tục mỗi frame để ktra + despawn enemies
        DespawnEnemies();
    }

    IEnumerator BeginNextWave()
    {
        // This is for the current wave
        isWaveActive = true;

        // Waits for set amount of time (usually 1 min) to start next wave
        yield return new WaitForSeconds(waveInterval);

        // We go to the next wave: (we use Count - 1 since we're incrementing, when we're at Count - 2 then it'd increment to Count - 1, thus settling us at the last idx)
        // + We also calculate the new quota
        if (currentWaveIdx < waves.Count - 1)
        {
            // Sau khi hết waveInterval, cho isWaveActive = false để qua wave mới
            isWaveActive = false;
            currentWaveIdx++;
            Debug.LogWarning("--- Moved to wave: " + waves[currentWaveIdx].waveName);
            // Cho modifiedSpawnInterval bằng spawnInterval của wave hiện tại
            //modifiedSpawnInterval = waves[currentWaveIdx].spawnInterval;

            //CalculateWaveQuota();
        }

    }

    //public void CalculateWaveQuota()
    //{
    //    int currentWaveQuota = 0;
    //    // Calculate current minimum number of enemies to spawn this wave
    //    foreach(var enemyGroup in waves[currentWaveIdx].enemyGroup)
    //    {
    //        currentWaveQuota += enemyGroup.minEnemyCount;
    //    }

    //    waves[currentWaveIdx].minWaveQuota = currentWaveQuota;

    //    Debug.LogWarning("Wave " + (int)(currentWaveIdx + 1) + " Quota: " + currentWaveQuota);
    //}

    /// <summary>
    /// This method will stop spawning enemies when the max number of enemies on the field has been reached.
    /// It will only spawn enemies in a particular wave until it is time for the next wave's enemies to be spawned.
    /// </summary>
    /// 
    /// waves[idx].currentSpawnCount is here for the sake of tracking, I don't really intend to use it in conditions.
    void SpawnEnemies()
    {
        // Basing on Notes
        if (!maxEnemiesReached)
        {
            // 1. Checking whether the minimum number of enemies of this current wave has been spawned
            if (currentEnemiesAlive < waves[currentWaveIdx].minWaveQuota)
            {
                Debug.LogWarning("--- SPAWN (1) ---");
                int randomMode = Random.Range(1, 3);        // Random Mode 1 or Mode 2
                // If there's only one enemy group in the current wave, mode 2 will always be picked
                if (waves[currentWaveIdx].enemyGroup.Count == 1)
                {
                    randomMode = 2;
                }

                /// Mode 1: Spawn each type of enemies until minWaveQuota is filled
                // Since we go through each type of enemy once, almost each type of enemy in the wave will be evenly spawned.
                if (randomMode == 1)
                {
                    // Spawn each type of enemy until minWaveQuota is filled
                    // If we have this while loop do we even need the modifiedSpawnInterval
                    //while (waves[currentWaveIdx].currentSpawnCount < waves[currentWaveIdx].minWaveQuota)
                    while (currentEnemiesAlive < waves[currentWaveIdx].minWaveQuota)
                    {
                        foreach (var enemyGroup in waves[currentWaveIdx].enemyGroup)
                        {
                            // Check if the minimum number of enemies of this type have been spawned

                            GameObject enemy = Instantiate(enemyGroup.enemyPrefab, GetSpawnPosition(enemyGroup.enemyPrefab), Quaternion.identity, enemyParent.transform);
                            enemy.tag = "Enemy";
                            enemy.layer = LayerMask.NameToLayer("Enemy");
                            // Calls OnEnemyKilled when object (enemy) is destroyed / when onDestroy event (Destroyable.cs) is Invoked
                            enemy.GetComponent<Destroyable>().onDestroy.AddListener(OnEnemyKilled);
                            // Calls IncreaseKillCount when enemy is DEFEATED by the player (onHealthZero event (Health.cs), since onDestroy event is used in Despawning)
                            enemy.GetComponent<Health>().onHealthZero.AddListener(IncreaseKillCount);
                            // Add to enemyList (for despawn check)
                            enemyList.Add(enemy);

                            // Update current enemy type's spawn count
                            //enemyGroup.currentEnemyCount++;
                            // Update current wave's spawn count
                            waves[currentWaveIdx].currentSpawnCount++;
                            // Update enemy on field count (will be used in 3. )
                            currentEnemiesAlive++;

                            // Ends this current spawn wave, waits for next interval to spawn normally (THIS IS WRONG, THIS SKIPS THE maxEnemies check --> 1 extra enemy will be spawned)
                            //if (currentEnemiesAlive >= waves[currentWaveIdx].minWaveQuota)
                            //    return;


                            /// 3. If the maximum number of enemies are on the field ==> Stop spawning
                            if (currentEnemiesAlive >= maxEnemiesAllowed)
                            {
                                maxEnemiesReached = true;
                                return;     // Stops spawning until there are less than max enemies on the field
                            }

                            // Ends this current spawn wave, waits for next interval to spawn normally
                            if (currentEnemiesAlive >= waves[currentWaveIdx].minWaveQuota)
                                break;
                        }
                    }
                    //
                }

                /// Mode 2: Spawn random enemies until minWaveQuota is filled
                // Again, do we even need the modifiedSpawnInterval???
                if (randomMode == 2)
                {
                    while (currentEnemiesAlive < waves[currentWaveIdx].minWaveQuota)
                    {
                        EnemyGroup enemyGroup_ = waves[currentWaveIdx].enemyGroup[Random.Range(0, waves[currentWaveIdx].enemyGroup.Count)];
                        GameObject enemy = Instantiate(enemyGroup_.enemyPrefab, GetSpawnPosition(enemyGroup_.enemyPrefab), Quaternion.identity, enemyParent.transform);
                        enemy.tag = "Enemy";
                        enemy.layer = LayerMask.NameToLayer("Enemy");
                        // Calls OnEnemyKilled when object (enemy) is destroyed / when onDestroy event is Invoked
                        enemy.GetComponent<Destroyable>().onDestroy.AddListener(OnEnemyKilled);
                        // Calls IncreaseKillCount when enemy is DEFEATED by the player (onHealthZero event (Health.cs), since onDestroy event is used in Despawning)
                        enemy.GetComponent<Health>().onHealthZero.AddListener(IncreaseKillCount);
                        // Add to enemyList (for despawn check)
                        enemyList.Add(enemy);

                        // Update current enemy type's spawn count
                        //enemyGroup_.currentEnemyCount++;
                        // Update current wave's spawn count
                        waves[currentWaveIdx].currentSpawnCount++;
                        // Update enemy on field count (will be used in 3. )
                        currentEnemiesAlive++;

                        // Ends this current spawn wave, waits for next interval to spawn normally (THIS IS WRONG, THIS SKIPS THE maxEnemies check --> 1 extra enemy will be spawned)
                        //if (currentEnemiesAlive >= waves[currentWaveIdx].minWaveQuota)
                        //    return;

                        /// 3. If the maximum number of enemies are on the field ==> Stop spawning
                        if (currentEnemiesAlive >= maxEnemiesAllowed)
                        {
                            maxEnemiesReached = true;
                            return;     // Stops spawning until there are less than max enemies on the field
                        }

                        // Ends this current spawn wave, waits for next interval to spawn normally
                        if (currentEnemiesAlive >= waves[currentWaveIdx].minWaveQuota)
                            break;
                    }
                }
            }

            // 2. If more enemies than the quota is present ==> spawn a (random) enemy periodically
            else
            {
                EnemyGroup enemyGroup_ = waves[currentWaveIdx].enemyGroup[Random.Range(0, waves[currentWaveIdx].enemyGroup.Count)];
                GameObject enemy = Instantiate(enemyGroup_.enemyPrefab, GetSpawnPosition(enemyGroup_.enemyPrefab), Quaternion.identity, enemyParent.transform);
                enemy.tag = "Enemy";
                enemy.layer = LayerMask.NameToLayer("Enemy");
                // Calls OnEnemyKilled when object (enemy) is destroyed / when onDestroy event is Invoked
                enemy.GetComponent<Destroyable>().onDestroy.AddListener(OnEnemyKilled);
                // Calls IncreaseKillCount when enemy is DEFEATED by the player (onHealthZero event (Health.cs), since onDestroy event is used in Despawning)
                enemy.GetComponent<Health>().onHealthZero.AddListener(IncreaseKillCount);
                // Add to enemyList (for despawn check)
                enemyList.Add(enemy);

                Debug.LogWarning("--- SPAWN (2) ---");

                // Update current enemy type's spawn count
                //enemyGroup_.currentEnemyCount++;
                // Update current wave's spawn count
                waves[currentWaveIdx].currentSpawnCount++;
                // Update enemy on field count (will be used in 3. )
                currentEnemiesAlive++;

                /// 3. If the maximum number of enemies are on the field ==> Stop spawning
                if (currentEnemiesAlive >= maxEnemiesAllowed)
                {
                    maxEnemiesReached = true;
                    return;     // Stops spawning until there are less than max enemies on the field
                }
            }
        }

        else
        {
            Debug.LogWarning("--- MAX ENEMIES REACHED ---");
        }

        // 3. Resets the maxEnemies reached flag if the number of enemies alive has dropped below the max amount
        //if (currentEnemiesAlive < maxEnemiesAllowed)
        //{
        //    maxEnemiesReached = false;
        //}
        /// Dời vào OnEnemyKilled để ta không phải tốn 1 lần qua if rồi Instantiate vì cụm đó mất tgian, làm delay flag
        /// maxEnemiesReached = false cũng ko cần phụ thuộc vào spawnTimer và SpawnEnemies (SpawnEnemies chỉ chạy sau 1 khoảng tgian)
        /// ==> Ta muốn giá trị false được cập nhật LIỀN (tại mọi thời điểm, ko phụ thuộc spawnTimer và spawnInterval),
        /// NGAY KHI Enemy is killed ==> Đưa vào OnEnemyKilled(), nó được gọi mỗi khi onDestroy được Invoked (và onDestroy được Invoked khi ta muốn xóa Enemy)
    }

    // Check if enemy is outside of the Despawn Area, if it is, then despawn it
    void DespawnEnemies()
    {
        if (enemyList.Count == 0)
        {
            return;
        }

        // Index cuối của số enemy để while loop check tới, nếu enemyIdxToCheck vượt thì ngưng while loop
        int checkTarget = enemyIdxToCheck + enemiesCheckPerFrame - 1;
        // Cái này sẽ ko đề cập tới việc checkTarget > enemyList.Count - 1

        while (enemyIdxToCheck <= checkTarget)
        {
            // Nếu idx hiện tại vẫn còn trong ngưỡng của List
            if (enemyIdxToCheck <= enemyList.Count - 1)
            {
                // Nếu enemy obj hiện tại trong list ko phải là null
                if (enemyList[enemyIdxToCheck] != null)
                {
                    // Nếu enemy nằm NGOÀI despawn area
                    // Xóa enemy đang xét
                    // ==> Bỏ khỏi List, đôn idx ktra lên (vì RemoveAt tự động đôn idx của các phần từ trong List)
                    // ==> Giữ nguyên giá trị của enemyIdxCheck để check với phần tử đc đôn lên
                    Vector3 playerPos = GetPlayerPosition();
                    if (enemyList[enemyIdxToCheck].transform.position.x > (playerPos.x + despawnRadius) ||
                        enemyList[enemyIdxToCheck].transform.position.x < (playerPos.x - despawnRadius) ||
                        enemyList[enemyIdxToCheck].transform.position.z > (playerPos.z + despawnRadius) ||
                        enemyList[enemyIdxToCheck].transform.position.z < (playerPos.z - despawnRadius))
                    {
                        enemyList[enemyIdxToCheck].GetComponent<Destroyable>().DestroyObject();
                        enemyList.RemoveAt(enemyIdxToCheck);
                        checkTarget--;
                    }
                    // Nếu enemy hiện tại vẫn còn trong despawn area => check enemy tiếp theo
                    else
                    {
                        enemyIdxToCheck++;
                    }
                }
                // THợp enemy bị killed thay vì despawned
                // ==> Bỏ khỏi List, đôn idx ktra lên (vì RemoveAt tự động đôn idx của các phần từ trong List)
                // ==> Giữ nguyên giá trị của enemyIdxCheck để check với phần tử đc đôn lên
                else
                {
                    enemyList.RemoveAt(enemyIdxToCheck);
                    checkTarget--;
                }
            }
            // Nếu đã tới cuối List => reset cả idx và checkTarget
            // ==> Lúc này while không còn hợp lệ --> ngưng, để cho lần gọi sau checkTarget đc tính lại
            // Đây là lý do ta ko đề cập tới việc checkTarget > enemyList.Count - 1. Vì tới cuối List hiện tại là ta đã reset nó trước khi nó vượt enemyList.count - 1
            else
            {
                enemyIdxToCheck = 0;
                checkTarget = 0 - 1;
            }
        }
    }

    /// <summary>
    /// Despawn ALL enemies (will be used at the end, right when entering the FINAL WAVE, but before SpawnWave runs)
    /// </summary>
    public void DespawnAllEnemies()
    {
        
        if (enemyList.Count == 0)
        {
            Debug.LogWarning("--- ALL ENEMIES HAVE BEEN DESPAWNED ---");
            resetAllEnemies = true;
            return;
        }

        Debug.LogWarning("--- DESPAWN ALL ENEMIES ---");

        // Index cuối của số enemy để while loop check tới, nếu enemyIdxToCheck vượt thì ngưng while loop
        int checkTarget = enemyIdxToCheck_ + enemiesCheckPerFrame - 1;
        // Cái này sẽ ko đề cập tới việc checkTarget > enemyList.Count - 1

        while (enemyIdxToCheck_ <= checkTarget)
        {
            // Nếu idx hiện tại vẫn còn trong ngưỡng của List
            if (enemyIdxToCheck_ <= enemyList.Count - 1)
            {
                // Nếu enemy obj hiện tại trong list ko phải là null
                if (enemyList[enemyIdxToCheck_] != null)
                {
                    // Về cơ bản, ta sẽ xóa liên tục tại enemyIdxToCheck_ = 0. Cho tới khi nào checkTarget giảm xuống còn -1.
                    // Khi ta removeAt thì idx của các phần tử trong List được tự động đôn lên, nên sau khi xóa thì phần tử tiếp theo (idx=1) sẽ được đôn lên làm 0
                    // và được xóa tới tới.
                    // Việc tới cuối List là ko thể tại ta ko increment enemyIdxToCheck.
                    // Sau khi checkTarget = -1 thì while kết thúc. Nếu enemyList.Count chưa == 0 thì DespawnAllEnemies được gọi ở frame tiếp theo.
                    enemyList[enemyIdxToCheck_].GetComponent<Destroyable>().DestroyObject();
                    enemyList.RemoveAt(enemyIdxToCheck_);
                    checkTarget--;

                    //enemyIdxToCheck++;
                }
                // THợp enemy bị killed thay vì despawned
                // ==> Bỏ khỏi List, đôn idx ktra lên (vì RemoveAt tự động đôn idx của các phần từ trong List)
                // ==> Giữ nguyên giá trị của enemyIdxCheck để check với phần tử đc đôn lên
                else
                {
                    enemyList.RemoveAt(enemyIdxToCheck_);
                    checkTarget--;
                }
            }
            /// === Trường hợp này sẽ ko bao giờ xảy ra trong hàm này btw ===
            // Nếu đã tới cuối List => reset cả idx và checkTarget
            // ==> Lúc này while không còn hợp lệ --> ngưng, để cho lần gọi sau checkTarget đc tính lại
            // Đây là lý do ta ko đề cập tới việc checkTarget > enemyList.Count - 1. Vì tới cuối List hiện tại là ta đã reset nó trước khi nó vượt enemyList.count - 1
            else
            {
                enemyIdxToCheck_ = 0;
                checkTarget = 0 - 1;
            }
        }
    }

    // Decrement enemy count when it is killed
    // This func will be called when an enemy is killed (and is put in onDestroy event (Destroyable.cs) since despawning also removes enemies alive)
    public void OnEnemyKilled()
    {
        currentEnemiesAlive--;
        waves[currentWaveIdx].currentSpawnCount--;

        /// 3. Resets the maxEnemies reached flag if the number of enemies alive has dropped below the max amount
        if (currentEnemiesAlive < maxEnemiesAllowed)
        {
            maxEnemiesReached = false;
        }
    }
    // Increment Total number of enemies killed count, this is applied for both Enemies and Bosses
    // This is put in a seperate func since Bosses aren't counted in currentEnemiesAlive and waves[currentWaveIdx].currentSpawnCount
    // (and is put in onHealthZero event (Health.cs) instead of onDestroy(Destroyable.cs))
    public void IncreaseKillCount()
    {
        totalEnemiesKilled++;
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

    // Resets count
    private void OnDestroy()
    {
        totalEnemiesKilled = 0;

        foreach(var wave in waves)
        {
            wave.ResetCount();
            //foreach(var enemyGroup in wave.enemyGroup)
            //{
            //    enemyGroup.ResetCount();
            //}
        }
    }

    private void OnApplicationQuit()
    {
        totalEnemiesKilled = 0;

        foreach (var wave in waves)
        {
            wave.ResetCount();
            //foreach (var enemyGroup in wave.enemyGroup)
            //{
            //    enemyGroup.ResetCount();
            //}
        }
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.cyan;
    //    Gizmos.DrawWireCube(player.transform.position, new Vector3(spawnRadius, 1f, spawnRadius));
    //}

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

// Make this a SO or sth
// We're making another class called EnemyGroup to add more info about each enemy type
//[System.Serializable]
//public class SpawnWave
//{
//    public string waveName;
//    public int waveMaxEnemy;            // The max number of enemies allowed in this wave
//    public List<EnemyGroup> enemyGroup; // List containing groups of enemies to spawn in this wave
//    public int waveQuota;               // The minimum number of enemies to spawn in this wave
//    public float spawnInterval;         // The interval between each enemy spawns
//    public int spawnCount;       // The current number of enemies in this wave
//}

//[System.Serializable]
//// We'd have to use this to manage each type of enemies' properties, since each may or may not have different HP or sth of the sort
//public class EnemyGroup
//{
//    public string enemyName;
//    public int maxEnemyCount;       // Max Number of enemies of this type to spawn in this wave
//    public int currentEnemyCount;   // Number of enemies of this type that are currently on the field
//    public GameObject enemyPrefab;  // Might add bullet pattern or attack type here
//}
