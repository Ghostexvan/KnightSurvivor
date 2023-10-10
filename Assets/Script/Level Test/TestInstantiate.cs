using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInstantiate : MonoBehaviour
{
    public GameObject testSpawn;
    //public GameObject parent;

    // Start is called before the first frame update
    void Start()
    {
        float yOffset = testSpawn.transform.GetChild(0).gameObject.GetComponent<Renderer>().bounds.extents.y;
        GameObject lemans = Instantiate(testSpawn, new Vector3(2, yOffset, 0), Quaternion.identity);
        //lemans.transform.parent = parent.transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
