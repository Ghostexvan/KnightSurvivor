using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

// Add a spawn manager to this
[RequireComponent(typeof(Terrain))]
public class TreeGenerator : MonoBehaviour
{
    [Header("=====Terrain Generation Variables=====")]
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

    [Header("Trees Prefab List")]
    [Tooltip("Prefabs with pivot on the ground and normal center same are recommended")]
    public List<GameObject> treePrefab;

    [Tooltip("Parent Empty Object, used to contain all the prefabs spawned")]
    public GameObject TreeGen;

    [Tooltip("Perlin Noise's acceptance value, is used to configure prefab spawning density")]
    [Range(0.8f, 1f)]
    public float acceptancePoint = 0.95f;

    [Header("Environmental Objects")]
    [Tooltip("Is used for Raycasting, OverlapBox/Spheres")]
    public LayerMask groundLayer;
    // Prevents tree spawn in initial player cam
    [Tooltip("Is used to prevent tree spawn in initial player camera")]
    public Camera mainCamera;


    // Ray variable
    //private Ray ray;

    // Enables building spawn
    //public bool spawnStructures;
    // Enables small foliage spawn
    //public bool spawnFoliage;

    private void Awake()
    {
        //mainCamera = GameObject.Find("Camera").GetComponent<Camera>();
        // Có thể lấy đc vì nó là DL có sẵn trong Component; và ta lấy Component từ chính Terrain Object
        Terrain _data = GetComponent<Terrain>();
        width = Mathf.FloorToInt(_data.terrainData.size.x);
        length = Mathf.FloorToInt(_data.terrainData.size.z);
    }

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GameObject.Find("Camera").GetComponent<Camera>();
        //StartCoroutine(SpawnTrees());
    }

    public IEnumerator SpawnTrees()
    {
        //scale = Random.Range(0.1f, 10f);
        seed = Random.Range(0, 10);
        offset = 2;     // 
        //seed = 0;


        if (treePrefab.Count > 0)
        {
            for (int z = 0 + offset; z < length - offset; z++)
            {
                for (int x = 0 + offset; x < width - offset; x++)
                {
                    if (IsPositionOnCameraViewPort(new Vector3(x, 0, z))) {
                        continue;
                    }

                    float xValue = x / scale;
                    float zValue = z / scale;

                    float perlinValue = Mathf.PerlinNoise(xValue + seed, zValue + seed);
                    //print(perlinValue);
                    // With a random offset of 0.5, we can ensure that nothing will go out of our spawning area - I hope
                    // (which is Terrain width and length with an offset of 2)
                    float xRand = Random.Range((float)(x - 0.5), (float)(x + 0.5));
                    float zRand = Random.Range((float)(z - 0.5), (float)(z + 0.5));

                    if (perlinValue >= acceptancePoint && IsSpawnable(xRand, zRand))
                    {
                        float yPoint = 0f;
                        if (Physics.Raycast(new Vector3(xRand, 10, zRand), Vector3.down, out RaycastHit hit, 50f, groundLayer))
                        {
                            yPoint = hit.point.y;
                        }
                        GameObject tree = Instantiate(treePrefab[Random.Range(0, treePrefab.Count)], new Vector3(x, yPoint, z), Quaternion.Euler(0, Random.Range(-45, 45), 0));
                        tree.tag = "Tree";
                        tree.layer = LayerMask.NameToLayer("Foliage");
                        tree.transform.parent = TreeGen.transform;
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
            print("No tree prefabs added!");
        }
        yield return null;
    }

    // Fix tree clippng in land
    // Fix tree spawning in spawn point area (DONE)
    bool IsSpawnable(float xCoordInt, float zCoordInt)
    {

        //if (Physics.SphereCast(new Vector3(xCoordInt, 0, zCoordInt), 10f, Vector3.zero, out RaycastHit hit)) {
        //    if (hit.transform.CompareTag("Tree"))
        //    {
        //        print("Called it");
        //        return false;
        //    }
        //}

        for (float i = 0; i < 360; i += 5)
        {
            // If the tree hits anything within a 10f radius, it won't spawn
            if (Physics.Raycast(new Vector3(xCoordInt, 0.25f, zCoordInt), Quaternion.Euler(0, i, 0) * Vector3.forward, out RaycastHit hit, 10f))
            {
                return false;
            }
        }
        return true;
    }

    bool IsPositionOnCameraViewPort(Vector3 position)
    {
        if (mainCamera.WorldToViewportPoint(position).x >= 0 && mainCamera.WorldToViewportPoint(position).x <= 1 &&
            mainCamera.WorldToViewportPoint(position).y >= 0 && mainCamera.WorldToViewportPoint(position).y <= 1 &&
            mainCamera.WorldToViewportPoint(position).z >= 0)
            return true;
        return false;
    }
}
