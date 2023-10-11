using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class StructureLootSpawn : MonoBehaviour
{
    public GameObject chestPrefab;
    public float spawnChance = 0.5f;

    // Testing values (will be cleaned later)
    private float xExtents;
    private float zExtents;

    private GameObject collectableParent;

    private void Awake()
    {
        collectableParent = GameObject.Find("Collectable");
    }


    // Start is called before the first frame update
    void Start()
    {
        SpawnLoot();
        xExtents = this.transform.GetChild(0).gameObject.GetComponent<Renderer>().bounds.extents.x;
        zExtents = this.transform.GetChild(0).gameObject.GetComponent<Renderer>().bounds.extents.z;
        print(this.transform.GetChild(0).gameObject.GetComponent<Renderer>().bounds.size);
        
    }

    // Raycast test three baby
    private void OnDrawGizmosSelected()
    {
        Debug.DrawRay(new Vector3(transform.position.x + xExtents, 0, transform.position.z + zExtents), Vector3.up * 10f, Color.blue);
        Debug.DrawRay(new Vector3(transform.position.x + xExtents, 0, transform.position.z - zExtents), Vector3.up * 10f, Color.blue);
        Debug.DrawRay(new Vector3(transform.position.x - xExtents, 0, transform.position.z + zExtents), Vector3.up * 10f, Color.blue);
        Debug.DrawRay(new Vector3(transform.position.x - xExtents, 0, transform.position.z - zExtents), Vector3.up * 10f, Color.blue);
        Debug.DrawRay(transform.GetChild(0).gameObject.GetComponent<Renderer>().bounds.center, Vector3.up * 10f, Color.blue);
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


    void SpawnLoot()
    {
        if (spawnChance >= Random.Range(0f, 1f))
        {
            print("Chest spawned");
            GameObject spawnPos = transform.Find("Spawn Points").gameObject;
            if (spawnPos != null && chestPrefab != null)
            {
                int randomSpawnLocationIdx = Random.Range(0, spawnPos.transform.childCount);
                //for (int i = 0; i < spawnPos.transform.childCount; i++)
                //{
                //    print("------- ITERATION: " + i);
                //    print("World Pos: " + transform.TransformPoint(spawnPos.transform.GetChild(i).position));
                //    print("Rotato: " + spawnPos.transform.GetChild(i).rotation);  // .rotation is world rotation; .localRotation is local rota
                //}
                //Vector3 randPos = transform.TransformPoint(spawnPos.transform.GetChild(randomSpawnLocationIdx).position);

                // Taking the world Position and Rotation values of the spawn locations (localPos if oyu want to get the local pos)
                Vector3 randPos = spawnPos.transform.GetChild(randomSpawnLocationIdx).position;
                Quaternion randRota = spawnPos.transform.GetChild(randomSpawnLocationIdx).rotation;

                //chestPrefab.TryGetComponent<MeshRenderer>(out MeshRenderer meshRen) {

                //}
                //float chestHeight = chestPrefab.GetComponent<MeshRenderer>().bounds.size.y;

                // We want to chest to spawn right on the ground, normally if y=0 then it'd be buried halfway (lmao)
                //float halfChestHeight = chestPrefab.GetComponent<MeshRenderer>().bounds.extents.y;
                Bounds chestPrefabBounds = GetChildRendererBounds(transform.GetChild(0).gameObject);
                float halfChestHeight = chestPrefabBounds.extents.y;

                // We also want the chest to spawn according to the height of its spawn point's position, so we might need to add a bit more y values
                // For example, a chest can spawn on high platforms, right?
                // So we need to spawn the chest with half its height (in order for it to not clip in the ground), PLUS the height of the place where it was spawned.
                // If it's on the ground then the extra height would just be 0f.
                float yPoint = 0f;
                if (Physics.Raycast(new Vector3(randPos.x, 10, randPos.z), Vector3.down, out RaycastHit hit, 50f))
                {
                    yPoint = hit.point.y;
                }

                GameObject chest = Instantiate(chestPrefab, new Vector3(randPos.x, 0.1f + halfChestHeight + yPoint, randPos.z), randRota);
                //chest.transform.parent = transform;
                chest.transform.parent = collectableParent.transform;
            }
        }
        else
        {
            print("Better luck next time");
        }
    }

}
