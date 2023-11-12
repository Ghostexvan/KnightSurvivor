using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy Spawns/BossWave")]
[Serializable]
public class BossWave : ScriptableObject
{
    public string bossWaveName;

    [Header("List containing groups of Bosses to spawn in this wave")]
    public List<BossGroup> bossGroup; // List containing groups of enemies to spawn in this wave

    [Header("The number of bosses to spawn in this wave (CURRENTLY UNUSED FOR NOW)")]
    //[Tooltip("You don't need to edit this")]
    public int numberToSpawn;               // The number of bosses to spawn in this wave

    // We don't need spawnIntervals or currentSpawnCount because:
    // We only spawn them ONCE, that's it.
}

[Serializable]
public class BossGroup
{
    public string bossName;
    [Header("Minimum Number of enemies to spawn in this wave, to fill the wave quota")]
    //public int minEnemyCount;       // Minimum Number of enemies of this type to spawn in this wave
    //public int currentEnemyCount = 0;   // Number of enemies of this type that are currently on the field
    public GameObject bossPrefab;  // You just plug and play those bullet shooting scripts it, there's nothing more to it I think

    public BossGroup()
    {
        bossName = "Default Test Boss";
        //minEnemyCount = 100;
    }
}