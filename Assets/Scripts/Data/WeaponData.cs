using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Weapon")]
public class WeaponData : ScriptableObject
{
    public enum WeaponType
    {
        Projectile,
        Raycast,
        Beam
    }

    public WeaponType type;
    public string displayName;
    public Vector2 damageRange;
    public float cooldown;
    public float range;
    // Dot product of leniency to forward vector
    [Range(0,1)]
    public float trackingFOV;

    [Header("Prefabs")]
    public GameObject muzzleVFXPrefab;
    public GameObject onHitVFXPrefab;
    
    [ShowIf("type", WeaponType.Projectile)]
    public Projectile projectile;
    [ShowIf("type", WeaponType.Projectile)]
    public float projectileSpeed;
}
