using System;
using UnityEngine;

[System.Serializable]
public class ItemInstance{
    public ItemData itemType;
    public int currentLevel;
    
    public ItemInstance(ItemData itemType, int currentLevel = 1)
    {
        this.itemType = itemType;
        this.currentLevel = currentLevel;
    }

    public ItemInstance(ItemInstance other){
        itemType = other.itemType;
        currentLevel = other.currentLevel;
    }
    
    public void Equip(CharacterData data){
        itemType.isGet = true;
        for (int index = 0; index < itemType.itemStats.Count; index++){
            if (itemType.itemStats[index].level > currentLevel)
                return;

            switch (itemType.itemStats[index].statAffect){
                case StatType.health:
                    data.health.AddModifier(new StatModifier(itemType.itemStats[index].value, itemType.itemStats[index].statType, this));
                    break;
                case StatType.defense:
                    data.defense.AddModifier(new StatModifier(itemType.itemStats[index].value, itemType.itemStats[index].statType, this));
                    break;
                case StatType.attackDamage:
                    data.attackDamage.AddModifier(new StatModifier(itemType.itemStats[index].value, itemType.itemStats[index].statType, this));
                    break;
                case StatType.attackCooldown:
                    data.attackCooldown.AddModifier(new StatModifier(itemType.itemStats[index].value, itemType.itemStats[index].statType, this));
                    break;
                case StatType.attackCount:
                    data.attackCount.AddModifier(new StatModifier(itemType.itemStats[index].value, itemType.itemStats[index].statType, this));
                    break;
                case StatType.moveSpeed:
                    data.moveSpeed.AddModifier(new StatModifier(itemType.itemStats[index].value, itemType.itemStats[index].statType, this));
                    break;
                case StatType.critChance:
                    data.critChance.AddModifier(new StatModifier(itemType.itemStats[index].value, itemType.itemStats[index].statType, this));
                    break;
                case StatType.critDamage:
                    data.critDamage.AddModifier(new StatModifier(itemType.itemStats[index].value, itemType.itemStats[index].statType, this));
                    break;
                default:
                    break;
            }
        }
    }

    public void Unequip(CharacterData data){
        data.health.RemoveAllModifiersFromSource(this);
        data.defense.RemoveAllModifiersFromSource(this);
        data.attackDamage.RemoveAllModifiersFromSource(this);
        data.attackCooldown.RemoveAllModifiersFromSource(this);
        data.attackCount.RemoveAllModifiersFromSource(this);
        data.moveSpeed.RemoveAllModifiersFromSource(this);
        data.critChance.RemoveAllModifiersFromSource(this);
        data.critDamage.RemoveAllModifiersFromSource(this);
    }

    public void LevelUp(CharacterData data){
        Unequip(data);
        currentLevel += 1;
        Equip(data);
    }

    public int GetMaxLevel(){
        int maxLevel = 0;

        for (int index = 0; index < itemType.itemStats.Count; index++){
            maxLevel = Math.Max(maxLevel, itemType.itemStats[index].level);
        }

        Debug.Log("Max level: " + maxLevel);
        return maxLevel;
    }
}