using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Add a spawn manager to this
[RequireComponent(typeof(Terrain))]
public class FoliageGenerator : MonoBehaviour
{
    [Header("=====Foliage Generation Variables=====")]
    [Header("Width - Height of Terrain")]
    public int width;
    public int length;
    [Tooltip("Offset of Terrain width and height, used to limit prefab spawn area (Since prefab can spawn at the edge, making things look unnatural)")]
    public int offset;

    [Header("Scale and Seed of Perlin Noise")]
    //[Range(0.01f, 10f)]
    [Tooltip("The bigger this is, the bigger and more complicated the Perlin Noise will be. (It's pretty much like zooming out)")]
    public float scale = 10f;
    [Tooltip("Adds randomness to the Perlin Noise")]
    public int seed;

    [Header("Foliage Prefab List")]
    [Tooltip("Prefabs with pivot on the ground and normal center same are recommended")]
    public List<GameObject> foliagePrefab;

    [Tooltip("Parent Empty Object, used to contain all the prefabs spawned")]
    public GameObject FoliageGen;

    [Tooltip("Perlin Noise's acceptance value, is used to configure prefab spawning density")]
    [Range(0.8f, 1f)]
    public float acceptancePoint = 0.95f;

    [Header("Environmental Objects")]
    [Tooltip("Is used for Raycasting, OverlapBox/Spheres")]
    public LayerMask groundLayer;

    // Ray variable
    //private Ray ray;

    // Prevents tree spawn in initial player cam
    //public Camera mainCamera;

    // Enables building spawn
    //public bool spawnStructures;
    // Enables small foliage spawn
    //public bool spawnFoliage;

    private void Awake()
    {
        //mainCamera = GameObject.Find("Camera").GetComponent<Camera>();
        Terrain _data = GetComponent<Terrain>();
        width = Mathf.FloorToInt(_data.terrainData.size.x);
        length = Mathf.FloorToInt(_data.terrainData.size.z);
    }

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(SpawnFoliage());
    }

    public IEnumerator SpawnFoliage()
    {
        //scale = Random.Range(0.1f, 10f);
        if (scale == 0)
            scale = 10;
        seed = Random.Range(0, 10);
        offset = 2;     // 
        //seed = 0;


        if (foliagePrefab.Count > 0)
        {
            for (int z = 0 + offset; z < length - offset; z++)
            {
                for (int x = 0 + offset; x < width - offset; x++)
                {
                    if (IsOnSpawnPoint(x, z))
                    {
                        //print("Bush on spawn point");
                        continue;
                    }

                    float xValue = x / scale;
                    float zValue = z / scale;

                    float perlinValue = Mathf.PerlinNoise(xValue + seed, zValue + seed);

                    GameObject randBush = foliagePrefab[Random.Range(0, foliagePrefab.Count)];
                    //print(perlinValue);
                    Quaternion randRota = Quaternion.Euler(0, Random.Range(-45, 45), 0);
                    // With a random offset of 0.5, we can ensure that nothing will go out of our spawning area
                    // (which is Terrain width and length with an offset of 2)
                    float xRand = Random.Range((float)(x - 0.5), (float)(x + 0.5));
                    float zRand = Random.Range((float)(z - 0.5), (float)(z + 0.5));

                    if (perlinValue >= acceptancePoint && IsSpawnable(randBush, xRand, zRand, randRota))
                    {
                        float yPoint = 0f;
                        if (Physics.Raycast(new Vector3(x, 10, z), Vector3.down, out RaycastHit hit, 50f, groundLayer))
                        {
                            yPoint = hit.point.y;
                        }
                        GameObject bush = Instantiate(randBush, new Vector3(x, yPoint, z), randRota);
                        bush.tag = "Foliage";
                        bush.layer = LayerMask.NameToLayer("Foliage");
                        bush.transform.parent = FoliageGen.transform;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
        }
        else
        {
            print("No foliage prefabs added!");
        }
        yield return null;
    }

    // Fix tree clippng in land
    // Fix tree spawning in spawn point area
    bool IsSpawnable(GameObject prefab, float xCoordInt, float zCoordInt, Quaternion randRotation)
    {

        //if (Physics.SphereCast(new Vector3(xCoordInt, 0, zCoordInt), 10f, Vector3.zero, out RaycastHit hit)) {
        //    if (hit.transform.CompareTag("Tree"))
        //    {
        //        print("Called it");
        //        return false;
        //    }
        //}

        //for (float i = 0; i < 360; i += 5)
        //{
        //    if (Physics.Raycast(new Vector3(xCoordInt, 0.25f, zCoordInt), Quaternion.Euler(0, i, 0) * Vector3.forward, out RaycastHit hit, 10f))
        //    {
        //        if (hit.transform.CompareTag("Tree"))
        //        {
        //            //print("Called it");
        //            return false;
        //        }
        //    }
        //}

        // We take the boundaries of LOD0 (or any of them really)
        float xExtent = prefab.transform.GetChild(0).gameObject.GetComponent<Renderer>().bounds.extents.x;
        float yExtent = prefab.transform.GetChild(0).gameObject.GetComponent<Renderer>().bounds.extents.y;
        float zExtent = prefab.transform.GetChild(0).gameObject.GetComponent<Renderer>().bounds.extents.z;

        // If OverlapBox hits COLLIDERS aside from the Terrain, IsSpawnable will return false
        Collider[] hitColliders = Physics.OverlapBox(new Vector3(xCoordInt, yExtent, zCoordInt), new Vector3(xExtent + .1f, yExtent, zExtent + .1f), randRotation);

        foreach (Collider col in hitColliders)
        {
            if (col.transform.gameObject.layer != LayerMask.NameToLayer("Ground"))
            {
                //print("No bushes for you");
                return false;
            }
        }
        // Raycasts also needed COLLIDERS
        for (float i = 0; i < 360; i += 10)
        {
            if (Physics.Raycast(new Vector3(xCoordInt, 0.25f, zCoordInt), Quaternion.Euler(0, i, 0) * Vector3.forward, out RaycastHit hit, .65f))
            {
                if (hit.transform.CompareTag("Tree"))
                {
                    //print("Called it");
                    return false;
                }
            }
        }
        // Y'know what, let's just add Trigger Colliders to them, those prefabs

        return true;
    }

    //bool IsPositionOnCameraViewPort(Vector3 position)
    //{
    //    if (mainCamera.WorldToViewportPoint(position).x >= 0 && mainCamera.WorldToViewportPoint(position).x <= 1 &&
    //        mainCamera.WorldToViewportPoint(position).y >= 0 && mainCamera.WorldToViewportPoint(position).y <= 1 &&
    //        mainCamera.WorldToViewportPoint(position).z >= 0)
    //        return true;
    //    return false;
    //}

    bool IsOnSpawnPoint(float randX, float randZ)
    {
        if (Physics.Raycast(new Vector3(randX, 2f, randZ), Vector3.down, out RaycastHit hit, 3f))
        {
            if (hit.collider.name == "Spawn Point")
            {
                return true;
            }
        }
        return false;
    }
}
