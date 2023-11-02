using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestStat : MonoBehaviour
{
    public CharacterData data;
    public float health,
                 defense,
                 attackDamage,
                 attackCooldown,
                 attackCount,
                 moveSpeed,
                 critChance,
                 critDamage;
    // Start is called before the first frame update
    void Start()
    {
        health = data.health.Value;
        defense = data.defense.Value;
        attackDamage = data.attackDamage.Value;
        attackCooldown = data.attackCooldown.Value;
        attackCount = data.attackCount.Value;
        moveSpeed = data.moveSpeed.Value;
        critChance = data.critChance.Value;
        critDamage = data.critDamage.Value;
    }

    // Update is called once per frame
    void Update()
    {
        health = data.health.Value;
        defense = data.defense.Value;
        attackDamage = data.attackDamage.Value;
        attackCooldown = data.attackCooldown.Value;
        attackCount = data.attackCount.Value;
        moveSpeed = data.moveSpeed.Value;
        critChance = data.critChance.Value;
        critDamage = data.critDamage.Value;
    }
}
