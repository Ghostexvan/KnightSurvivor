using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterLevel : MonoBehaviour
{
    public int currentLevel,
               currentEXP,
               needEXP;
    public GameObject rewardChest;
    
    private void Awake() {
        currentLevel = 1;
        currentEXP = 0;
        needEXP = CalculateLevel();
    }

    void Update()
    {
        if (currentEXP >= needEXP)
            LevelUp();
    }

    int CalculateLevel(){
        if (currentLevel == 1)
            return 5;
        return 5 + (currentLevel * (int) Mathf.Log(currentLevel, 2));
    }

    void LevelUp(){
        currentEXP -= needEXP;
        currentLevel += 1;
        needEXP = CalculateLevel();
        Instantiate(rewardChest, transform.position + new Vector3(5.0f, 5.0f, 5.0f), Quaternion.identity);
    }
}
