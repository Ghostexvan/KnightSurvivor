using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastTestTwo : MonoBehaviour
{
    public int RaysOffset;
    public float RayLength;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmosSelected()
    {
        //Gizmos.color = Color.red;
        for (int i = 0; i < 360; i += RaysOffset)
        {
            Debug.DrawRay(new Vector3(transform.position.x, .25f, transform.position.z), Quaternion.Euler(0, i, 0) * Vector3.forward * RayLength, Color.blue);

        }
    }
}
