using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Weapon")]
public class WeaponData : ScriptableObject
{
    public string displayName;
    public float cooldown;
    public float range;
    // Dot product of leniency to forward vector
    [Range(0,1)]
    public float trackingFOV;
    public GameObject projectile;
    public float projectileSpeed;
}
