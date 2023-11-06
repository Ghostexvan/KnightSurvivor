using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.AI;
using UnityEngine;

//[RequireComponent(typeof(NavMeshSurface))]
public class GenerateNavMesh : MonoBehaviour
{
    // Getting Spawning Objects' parent objects
    public GameObject TreeGen;
    public GameObject StrucutureGen;
    public GameObject FoliageGen;

    // Navmesh(es) for Humanoid and Boss characters
    public NavMeshSurface humanoidNav;
    public NavMeshSurface bossNav;
    public NavMeshSurface humanoidBossNav;
    //public NavMeshSurface[]

    private void Awake()
    {
        //TreeGen = GameObject.Find("TreeGen");
        //StrucutureGen = GameObject.Find("StructureGen");
        //FoliageGen = GameObject.Find("FoliageGen");

        //humanoidNav = GameObject.Find("Humanoid Navmesh").GetComponent<NavMeshSurface>();
        //bossNav = GameObject.Find("Boss Navmesh").GetComponent<NavMeshSurface>();
    }

    // Start is called before the first frame update
    void Start()
    {
        TreeGen = GameObject.Find("TreeGen");
        StrucutureGen = GameObject.Find("StructureGen");
        FoliageGen = GameObject.Find("FoliageGen");

        humanoidNav = GameObject.Find("Humanoid Navmesh").GetComponent<NavMeshSurface>();
        bossNav = GameObject.Find("Boss Navmesh").GetComponent<NavMeshSurface>();
        humanoidBossNav = GameObject.Find("Humanoid Boss Navmesh").GetComponent<NavMeshSurface>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator CreateNavmesh()
    {
        // Change Structure's Override Area -> Area Type to NOT WALKABLE since we don't want enemeies to just walk on the structure's roofs
        NavMeshModifier strucutureModifier = StrucutureGen.AddComponent(typeof(NavMeshModifier)) as NavMeshModifier;
        strucutureModifier.overrideArea = true;
        strucutureModifier.area = 1;    // 1 is "Not Walkable"

        // After that, we bake each of the areas for their respective Agent Types
        humanoidNav.BuildNavMesh();
        bossNav.BuildNavMesh();
        humanoidBossNav.BuildNavMesh();

        yield return null;
    }

    private void OnDestroy()
    {
        // Delete Navmesh when scene is finished
        humanoidNav.RemoveData();
        bossNav.RemoveData();
        humanoidBossNav.RemoveData();
    }

    private void OnApplicationQuit()
    {
        // Delete Navmesh when exited from app
        humanoidNav.RemoveData();
        bossNav.RemoveData();
        humanoidBossNav.RemoveData();
    }
}
