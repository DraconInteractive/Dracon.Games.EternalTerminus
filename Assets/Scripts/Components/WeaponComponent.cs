using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Sirenix.OdinInspector;
public class WeaponComponent : ShipComponent
{
    public WeaponData data;
    public WeaponAim aim;
    [ReadOnly]
    public float currentCooldown;
    public Transform firePoint;

    private TargetController targetControl;

    private void Start()
    {
        targetControl = TargetController.Instance;
        aim.Setup(this);
    }

    private void Update()
    {
        if (currentCooldown > 0)
        {
            currentCooldown -= Time.deltaTime;
        }
        aim.UpdateAim(targetControl.trackedTarget.Item1, targetControl.state);
    }

    private Coroutine inputDownRoutine;
    public void OnInputDown()
    {
        if (inputDownRoutine != null)
        {
            StopCoroutine(inputDownRoutine);
        }

        inputDownRoutine = StartCoroutine(InputDownRoutine());
    }

    IEnumerator InputDownRoutine ()
    {
        while (true)
        {
            TryFire();
            yield return new WaitForEndOfFrame();
        }
    }

    public void OnInputRelease()
    {
        if (inputDownRoutine != null)
        {
            StopCoroutine(inputDownRoutine);
        }
    }
    
    public void TryFire()
    {
        if (currentCooldown <= 0)
        {
            Fire();
        }
    }

    public void Fire()
    {
        currentCooldown = data.cooldown;
        var go = Instantiate(data.projectile, firePoint.position, firePoint.rotation);
        var projectile = go.GetComponentInChildren<Projectile>();
        projectile.movementSpeed += Player.Instance.currentShip.Speed;
        projectile.gameObject.layer = LayerMask.NameToLayer("Player");
        projectile.movementSpeed = data.projectileSpeed;
    }

    private void OnDrawGizmos()
    {
        if (data != null)
        {
            if (firePoint != null)
            {
                //Gizmos.DrawRay(new Ray(firePoint.position, firePoint.forward * data.range));
                Gizmos.DrawLine(firePoint.position, firePoint.position + firePoint.forward * data.range);
            }
        }
    }
}
