using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawn : MonoBehaviour
{
    public List<GameObject> itemSpawn = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        Terrain terrain = GetComponent<Terrain>();
        GameObject item = itemSpawn[Random.Range(0, itemSpawn.Count)];
        //Instantiate(item, new Vector3(Random.Range(-terrain.terrainData.size.x, terrain.terrainData.size.x), terrain.terrainData.size.y, Random.Range(-terrain.terrainData.size.z, terrain.terrainData.size.z)), Quaternion.identity);

        if (terrain)
            print(terrain.terrainData.size.x);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
