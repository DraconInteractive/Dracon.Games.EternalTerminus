using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;
public class WeaponComponent : ShipComponent
{
    public WeaponData data;
    public WeaponAim aim;
    [ReadOnly]
    public float currentCooldown;
    public Transform firePoint;
    
    private void Start()
    {
        aim.Setup(this);
    }

    private void Update()
    {
        if (currentCooldown > 0)
        {
            currentCooldown -= Time.deltaTime;
        }
        aim.UpdateAim(_anchor.ship.trackedTarget.Item1, _anchor.ship.targetingState);
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

        Instantiate(data.muzzleVFXPrefab, firePoint.position, firePoint.rotation);
        if (data.type == WeaponData.WeaponType.Projectile)
        {
            var projectile = Instantiate(data.projectile, firePoint.position, firePoint.rotation);
            projectile.movementSpeed += Player.Instance.currentShip.Speed;
            projectile.gameObject.layer = LayerMask.NameToLayer("Player");
            projectile.movementSpeed = data.projectileSpeed;
        }
        else if (data.type == WeaponData.WeaponType.Raycast)
        {
            var ray = new Ray(firePoint.position, firePoint.forward);
            var hit = new RaycastHit();
            if (Physics.Raycast(ray, out hit, data.range))
            {
                if (hit.transform.TryGetComponent(out Pawn pawn))
                {
                    float damage = Random.Range(data.damageRange.x, data.damageRange.y);
                    pawn.OnHit(_anchor.ship, damage);
                    Instantiate(data.onHitVFXPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                }
            }
        }

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
