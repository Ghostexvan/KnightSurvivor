using UnityEngine;

[CreateAssetMenu]
public class WeaponData : ItemData
{
    public WeaponType weaponType;
    public HandleType handleType;
    public GameObject model;
    public AnimatorOverrideController animator;
}

public enum WeaponType{
    Dagger,
    Mace,
    Pistol,
    Spear,
    Sword,
    Axe,
    Bow,
    Crossbow,
    Shooting,
    Staff
};

public enum HandleType{
    LeftHand,
    RightHand,
    BothHand
};