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
                         attackCount;
    [Header("(Average Enemy Movement is around 3.0-3,5-ish btw)")]
    public CharacterStat moveSpeed;
    public CharacterStat critChance,
                         critDamage;

    [Header("Condition check")]
    public bool isOnGround,
                isJump,
                isMove,
                isAttack;

    [Header("Current State of this character")]
    public string currentState;
    public Direction moveDirection;

    [Header("This part is solely for enemies/bosses. You don't need these values in the Player's data.")]
    [Header("Enemies' skills (Only 1 will be added for now)")]
    [Tooltip("Multiplies the enemy's HP based on the Player's level - (HP * Level).\n" +
      "This is applied THE MOMENT THE ENEMY WAS SPAWNED, and WILL NOT BE UPDATED\n" +
      "in case the Player gains level while the current enemy is alive.")]
    public bool isHPScale;
}
