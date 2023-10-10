using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastDebug : MonoBehaviour
{
    // Testing values (will be cleaned later)
    Bounds allChildrenBounds;
    private float xExtents;
    private float yExtents;
    private float zExtents;

    public float radius;

    private void Awake()
    {
        allChildrenBounds = GetChildRendererBounds(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (transform.childCount > 0)
        {
            
            //xExtents = this.transform.GetChild(0).gameObject.GetComponent<Renderer>().bounds.extents.x;
            //yExtents = this.transform.GetChild(0).gameObject.GetComponent<Renderer>().bounds.extents.y;
            //zExtents = this.transform.GetChild(0).gameObject.GetComponent<Renderer>().bounds.extents.z;
            xExtents = allChildrenBounds.extents.x;
            yExtents = allChildrenBounds.extents.y;
            zExtents = allChildrenBounds.extents.z;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(new Vector3(transform.position.x, 0, transform.position.z), radius);
        Gizmos.DrawWireCube(transform.position, new Vector3(2*xExtents, 2*yExtents, 2*zExtents));
    }

    private void OnDrawGizmosSelected()
    {
        // OverlapBox Visualization
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(new Vector3(this.transform.position.x, 1f, this.transform.position.z), new Vector3(2*xExtents, 1f, 2*zExtents));
        //Gizmos.DrawWireCube(new Vector3(this.transform.position.x, 0, this.transform.position.z), new Vector3(2*xExtents, 0.5f, 2*zExtents));
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
}
