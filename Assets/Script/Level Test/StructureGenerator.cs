using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Terrain))]
public class StructureGenerator : MonoBehaviour
{
    [Header("=====Structures Generation Variables=====")]
    [Header("Width - Height of Terrain")]
    // Terrain area + offset in order to not 
    public int width;
    public int length;
    [Tooltip("Offset of Terrain width and height, used to limit prefab spawn area (Since prefab can spawn at the edge, making things look unnatural)")]
    public int offset = 2;

    [Tooltip("Parent Empty Object, used to contain all the prefabs spawned")]
    public GameObject ParentObject;

    [Tooltip("Number of structures to spawn")]
    // Number of structures to spawn
    [Range(1, 10)]
    public int numStructures;

    [Tooltip("Number of placement tries before changing into next structure")]
    // Number of placement tries before changing into next structure
    [Range(2, 100)]
    public int tries;

    [Tooltip("Number of structure rotation tries before settling with rotation = 0")]
    [Range(2, 10)]
    public int rotaTries;

    [Header("Structure Prefab List")]
    [Tooltip("Prefabs with pivot and center in the same are recommended")]
    // Structure prefab list
    public List<GameObject> structuresPrefab;

    // Random Pos variables
    private float randX;
    private float randZ;
    private Quaternion randRota;

    // Structure Blocked Area
    private GameObject structureBlockedArea;

    // Is able to hae random rotation
    private bool isRotaAllowed;

    private void Awake()
    {
        Terrain _data = GetComponent<Terrain>();
        width = Mathf.FloorToInt(_data.terrainData.size.x);
        length = Mathf.FloorToInt(_data.terrainData.size.z);

        //numStructures = Random.Range(1, 10 + 1);
        //structureBlockedArea = GameObject.Find("Struct Block Area");
        //structureBlockedArea.SetActive(true);

    }

    // Start is called before the first frame update
    void Start()
    {
        //isRotaAllowed = false;
        //if (structuresPrefab.Count > 0)
        //{
        //    // These will return the World coord in the "Isolated Prefab" space, simply put, you need it to be initialized in the world
        //    // One workaround is to simply have the Prefab spawn with the center of it being right at the random point (Changin its pivot point)
        //    if (structuresPrefab[0].transform.childCount > 0)
        //    {
        //        if (structuresPrefab[0].transform.GetChild(0).name == "Building") NOT REALLY NECESSARY SINCE I WANTED THE NAMING TO NOT BE FORCED AS "Building"
        //            print("Center: " + structuresPrefab[0].transform.GetChild(0).gameObject.GetComponent<Renderer>().bounds.center);
        //        else
        //            print("Center: " + structuresPrefab[0].transform.gameObject.GetComponent<Renderer>().bounds.center);
        //    }
        //    else
        //    {
        //        print("Center: " + structuresPrefab[0].transform.gameObject.GetComponent<Renderer>().bounds.center);
        //    }
        //    //
        //}
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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

    // Spawn Structures
    public IEnumerator SpawnStructures()
    {
        if (structuresPrefab.Count > 0)
        {
            // Loopthrough number of spawns
            for (int i = 0; i < numStructures; i++)
            {
                print("Num struct loop");
                int j = 0;

                // Kind reminder that
                // All structure prefab must follow the same format of having:
                // An EMPTY Parent, with a normal mesh+renderer+collider child
                GameObject structure = structuresPrefab[Random.Range(0, structuresPrefab.Count)];
                /// CALCULATE THE BOUNDS OF the current GameObject (We needed to do this since some prefabs have multiple renderers)
                Bounds currentBounds = GetChildRendererBounds(structure);

                //float yExtent = structure.transform.GetChild(0).gameObject.GetComponent<Renderer>().bounds.extents.y;   // First and only child of the prefab
                float yExtent = currentBounds.extents.y;
                randRota = Quaternion.Euler(0, transform.rotation.y, 0);
                do 
                {
                    randX = Random.Range(0 + offset, width - offset);
                    randZ = Random.Range(0 + offset, length - offset);
                    j++;
                } while (!GoodCoordBoolCheck(structure, randX, randZ, currentBounds) && j <= tries);

                if (j > tries)
                {
                    --i;
                    continue;
                }
                else
                {
                    int k = 0;
                    randRota = Quaternion.Euler(0, transform.rotation.y, 0);
                    do
                    {
                        randRota = Quaternion.Euler(0, transform.rotation.y + Random.Range(-25f, 25f), 0);
                        k++;
                    } while (IsRotateCollide(structure, randX, randZ, randRota, currentBounds) && k <= rotaTries);
                    if (k > rotaTries)
                    {
                        randRota = Quaternion.Euler(0, transform.rotation.y, 0);
                    }
                    //if (IsRotateCollide(structure, randX, randZ, randRota))
                    //{

                    //}
                    GameObject structPlacement = Instantiate(structure, new Vector3(randX, yExtent, randZ), randRota);
                    structPlacement.layer = LayerMask.NameToLayer("Structures");
                    structPlacement.transform.parent = ParentObject.transform;
                }
            }

            //structureBlockedArea.SetActive(false);

        }
        else
        {
            print("No structure prefabs added!");
            //structureBlockedArea.SetActive(false);
        }
        yield return null;
    }

    //bool GoodCoordBoolCheck(GameObject prefab, float randX, float randZ)
    //{
    //    if (IsAllowedCoords(randX, randZ) && !IsCollide(prefab, randX, randZ) && IsInBoundaries(prefab, randX, randZ))
    //    {
    //        return true;
    //    }
        

    //    return false;
    //}

    bool GoodCoordBoolCheck(GameObject prefab, float randX, float randZ, Bounds bounds)
    {
        if (!IsAllowedCoords(randX, randZ))
        {
            return false;
        }

        if (IsCollide(prefab, randX, randZ, bounds))
        {
            return false;
        }

        if (!IsInBoundaries(prefab, randX, randZ, bounds))
        {
            return false;
        }

        return true;
    }

    // Check if random coord is allowed for struct spawning
    // This will be done by firing a ray onto the ground, if it collides with "Struct Blocked Area" then it will try to get another coord
    bool IsAllowedCoords(float randX, float randZ)
    {
        if (Physics.Raycast(new Vector3(randX, 10, randZ), Vector3.down, out RaycastHit hit, 20f)) {
            if (hit.transform.name == "Struct Blocked Area")
            {
                print("Nuh uh! This is the spawing area.");
                return false;
            }
        }
        return true;
    }

    // Check if building collides with trees and foliage
    bool IsCollide(GameObject prefab, float randX, float randZ, Bounds bounds)
    {
        //float xExtents = prefab.transform.GetChild(0).gameObject.GetComponent<Renderer>().bounds.extents.x;
        //float yExtents = prefab.transform.GetChild(0).gameObject.GetComponent<Renderer>().bounds.extents.y;
        //float zExtents = prefab.transform.GetChild(0).gameObject.GetComponent<Renderer>().bounds.extents.z;

        float xExtents = bounds.extents.x;
        float yExtents = bounds.extents.y;
        float zExtents = bounds.extents.z;

        // Call OverlapBox (We can have this be as big as the structure we're trying to spawn, or just use the x and z extents of it)
        // We'll be using the x and z extents.
        // We want to see if there's any Foliage/Trees that collided with OverlapBox, if there is then we will simply return true
        Collider[] hitColliders = Physics.OverlapBox(new Vector3(randX, 1f, randZ), new Vector3(xExtents, 5f, zExtents), Quaternion.identity, LayerMask.GetMask("Foliage"));
        print("Tree cols: " + hitColliders.Length);
        if (hitColliders.Length > 0)
        {
            print("--- Trees found");
            return true;
        }
        foreach (Collider collider in hitColliders) {
            if (collider.CompareTag("Tree"))
            {
                print("--- Trees found");
                return true;
            }
        }

        //Collider[] result = new Collider[10];
        //int colliderCount = Physics.OverlapBoxNonAlloc(new Vector3(randX, yExtents, randZ), new Vector3(xExtents, yExtents, zExtents), result, Quaternion.identity, LayerMask.NameToLayer("Foliage"));
        //if (colliderCount > 0 || result.Length > 0)
        //{
        //    print("--- Trees found");
        //    return true;
        //}

        // Call OverlapSphere, from current random point with a radius of around 25-50 or so, to check for any nearby buildings
        Collider[] result_ = Physics.OverlapSphere(new Vector3(randX, 0, randZ), 50f, LayerMask.GetMask("Structures"));
        print("Building cols: " + result_.Length);
        if (result_.Length > 0)
        {
            print("--- Buildings found");
            return true;
        }
        //Collider[] result_ = new Collider[5];
        //int buildingCount = Physics.OverlapSphereNonAlloc(new Vector3(randX, 0, randZ), 50f, result_, LayerMask.NameToLayer("Structures"));
        //if (buildingCount > 0 || result_.Length > 0)
        //{
        //    print("--- Buildings found");
        //    return true;
        //}


        print("--- No collisions!");
        return false;
    }

    // Similar check but with bigger area, if this passes the check, the structure can rotate randomly
    bool IsRotateCollide(GameObject prefab, float randX, float randZ, Quaternion randAngle, Bounds bounds)
    {
        //float xExtents = prefab.transform.GetChild(0).gameObject.GetComponent<Renderer>().bounds.extents.x;
        //float zExtents = prefab.transform.GetChild(0).gameObject.GetComponent<Renderer>().bounds.extents.z;
        float xExtents = bounds.extents.x;
        float zExtents = bounds.extents.z;

        // Call OverlapBox (We can have this be as big as the structure we're trying to spawn, or just use the x and z extents of it)
        // We'll be using the x and z extents.
        // We want to see if there's any Foliage/Trees that collided with OverlapBox, if there is then we will simply return true
        //Collider[] hitColliders = Physics.OverlapBox(new Vector3(randX, 1, randZ), new Vector3(xExtents, 0.25f, zExtents), Quaternion.identity, LayerMask.NameToLayer("Foliage"));
        Collider[] result = new Collider[10];
        int colliderCount = Physics.OverlapBoxNonAlloc(new Vector3(randX, 1f, randZ), new Vector3(xExtents + 2f, 1f, zExtents + 2f), result, randAngle, LayerMask.GetMask("Foliage"));
        if (colliderCount > 0)
        {
            print("--- Trees found 2");
            return true;
        }

        return false;
    }

    // Check if 4 of building's corners are inside the Terrain/Border
    bool IsInBoundaries(GameObject prefab, float randX, float randZ, Bounds bounds)
    {
        float xExtent, zExtent;
        //if (prefab.transform.GetChild(0).name == "Building" && structuresPrefab[0].transform.childCount > 0)
        //{
        //    xBound = this.transform.GetChild(0).gameObject.GetComponent<Renderer>().bounds.extents.x;
        //    zBound = this.transform.GetChild(0).gameObject.GetComponent<Renderer>().bounds.extents.z;
        //}
        //else
        //{
        //    xBound = this.transform.GetComponent<Renderer>().bounds.extents.x;
        //    zBound = this.transform.GetComponent<Renderer>().bounds.extents.z;
        //}

        //xExtent = this.transform.GetChild(0).gameObject.GetComponent<Collider>().bounds.extents.x;
        //zExtent = this.transform.GetChild(0).gameObject.GetComponent<Collider>().bounds.extents.z;

        xExtent = bounds.extents.x;
        zExtent = bounds.extents.z;

        List<Vector3> cornerCoords = new List<Vector3>() {
            new Vector3(randX - xExtent, 10f, randZ - zExtent),
            new Vector3(randX - xExtent, 10f, randZ + zExtent),
            new Vector3(randX + xExtent, 10f, randZ - zExtent),
            new Vector3(randX + xExtent, 10f, randZ + zExtent)
        };
        //cornerCoords.Add(new Vector3(randX - xExtent, 10f, randZ - zExtent));
        //cornerCoords.Add(new Vector3(randX - xExtent, 10f, randZ + zExtent));
        //cornerCoords.Add(new Vector3(randX + xExtent, 10f, randZ - zExtent));
        //cornerCoords.Add(new Vector3(randX + xExtent, 10f, randZ + zExtent));

        //cornerCoords.Add(new Vector3(randX - xExtent, 10f, randZ - zExtent));
        //cornerCoords.Add(new Vector3(randX - xExtent, 10f, randZ + zExtent));
        //cornerCoords.Add(new Vector3(randX + xExtent, 10f, randZ - zExtent));
        //cornerCoords.Add(new Vector3(randX + xExtent, 10f, randZ + zExtent));


        foreach (Vector3 coords in cornerCoords)
        {
            print("Checking corner coord: " + coords);
            if (Physics.Raycast(coords, Vector3.down, out RaycastHit hit, 20f))
            {
                if (hit.collider != null)
                    continue;
            }
            else
            {
                return false;
            }
        }

        return true;
    }

    // Check if there's any buildings nearby (not really needed tho, and this should be combined with IsCollide up there)
}
