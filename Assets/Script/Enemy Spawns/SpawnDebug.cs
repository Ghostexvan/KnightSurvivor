using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SpawnDebug : MonoBehaviour
{
    public GameObject player;
    [Header("Adjust these variables to visualize the enemies' spawn radius and despawn radius")]
    [Tooltip("The radius of this will be the spawn radius of the EnemySpawnController")]
    public float spawnDiameter;
    [Tooltip("The radius of this will be the despawn radius of the EnemySpawnController")]
    public float despawnDiameter;

    private void Awake()
    {
        player = GameObject.Find("Player");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(player.transform.position, new Vector3(spawnDiameter, 1f, spawnDiameter));

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(player.transform.position, new Vector3(despawnDiameter, 1f, despawnDiameter));
    }
}
