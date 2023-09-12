using UnityEngine;

public class CollectableItem : MonoBehaviour
{
    public ItemInstance itemInstance;
    public float rotateSpeed;

    private void Awake() {
        itemInstance = new ItemInstance(itemInstance.itemType);
    }

    private void FixedUpdate() {
        if (IsOnGround()){
            gameObject.GetComponent<Rigidbody>().isKinematic = true;
        }
        else
            gameObject.GetComponent<Rigidbody>().isKinematic = false;
    }
    
    private void LateUpdate() {
        Rotate();
    }

    public ItemInstance Collect(){
        Destroy(gameObject);
        return itemInstance;
    }

    private bool IsOnGround()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1f);
    }

    private void Rotate(){
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
    }
}

[System.Serializable]
public class ItemInstance{
    public ItemData itemType;
    public int currentLevel;
    
    public ItemInstance(ItemData itemType)
    {
        this.itemType = itemType;
        currentLevel = 1;
    }

    
    public void Equip(CharacterData data){
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
}