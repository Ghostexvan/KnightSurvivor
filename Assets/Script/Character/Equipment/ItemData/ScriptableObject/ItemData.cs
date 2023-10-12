using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ItemData : ScriptableObject
{
    [Serializable]
    public struct itemStat{
        public int level;
        public StatType statAffect;
        public StatModType statType;
        public float value;
    }

    public string itemName;
    [TextArea]
    public string description;
    public Sprite icon;
    public List<itemStat> itemStats = new List<itemStat>();
    public bool isGet;
}
