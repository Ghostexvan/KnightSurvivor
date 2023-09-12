using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CharacterData : ScriptableObject
{   
    public CharacterStat health,
                         defense,
                         attackDamage,
                         attackCooldown,
                         attackCount,
                         moveSpeed,
                         critChance,
                         critDamage;

    [Header("Condition check")]
    public bool isOnGround,
                isJump,
                isMove,
                isAttack;

    [Header("Current State of this character")]
    public string currentState;
    public Direction moveDirection;
}
