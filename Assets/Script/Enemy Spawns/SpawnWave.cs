using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy Spawns/SpawnWave")]
[Serializable]
public class SpawnWave : ScriptableObject
{
    /// <summary>
    /// I'm basing our spawning system on this specifically: 
    /// https://vampire-survivors.fandom.com/wiki/Enemies#Waves
    /// I'll work on the boss soon, this will do for now
    /// </summary>

    public string waveName;
    //[Header("The max number of enemies allowed in this wave")]
    //[Tooltip("When enemy count reaches this point, no more will spawn; Except bosses and/or event spawns (swarms/hordes)")]
    //public int waveMaxEnemy;            // The max number of enemies allowed in this wave
    /// <summary>
    /// waveMaxEnemy is removed and replaced with maxAllowedEnemy in EnemySpawnController.cs, since it's a global var - not wave-specific
    /// </summary>

    [Header("List containing groups of enemies to spawn in this wave")]
    public List<EnemyGroup> enemyGroup; // List containing groups of enemies to spawn in this wave
    //[Header("The minimum number of enemies to spawn in this wave, will be calculated based on minEnemyCount of each group")]
    [Header("The minimum number of enemies to spawn in this wave")]
    //[Tooltip("You don't need to edit this")]
    public int minWaveQuota;               // The minimum number of enemies to spawn in this wave
    // If more enemies than the minWaveQuota is present => one of each type of enemies in the wave will spawn periodically
    [Header("The interval between each enemy spawns")]
    public float spawnInterval;         // The interval between each enemy spawns
    [Header("Current number of enemies in this wave, will be calculated automatically.\n" +
        "This will ONLY be used as cond. to change Waves. You do not need to edit this.")]
    public int currentSpawnCount = 0;       // The current number of enemies in this wave. This will ONLY be used as cond. to change waves.

    public void ResetCount()
    {
        currentSpawnCount = 0;
        //minWaveQuota = 0;
    }
}

[Serializable]
public class EnemyGroup
{
    public string enemyName;
    [Header("Minimum Number of enemies to spawn in this wave, to fill the wave quota")]
    //public int minEnemyCount;       // Minimum Number of enemies of this type to spawn in this wave
    //public int currentEnemyCount = 0;   // Number of enemies of this type that are currently on the field
    public GameObject enemyPrefab;  // Might add bullet pattern or attack type here

    public EnemyGroup()
    {
        enemyName = "Default Test Enemy";
        //minEnemyCount = 100;
    }

     public void ResetCount()
    {
        //currentEnemyCount = 0;
    }
}