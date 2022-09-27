using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAim : MonoBehaviour
{
    public float rotateSpeed, maxAngle;
    private Transform cachedTransform;
    private Quaternion startRot;
    private TargetController targetControl;
    private WeaponComponent component;

    public void Setup(WeaponComponent _component)
    {
        cachedTransform = this.transform;
        startRot = cachedTransform.localRotation;
        targetControl = TargetController.Instance;
        component = _component;
    }

    public void UpdateAim(BaseTargetable target, TargetController.TargetState state)
    {
        if (target != null && target is BaseEnemy && state == TargetController.TargetState.TargetLocked)
        {
            Quaternion targetRot = Quaternion.identity;
            float dist = Vector3.Distance(transform.position, target.Position());
            int predictionFrames = Mathf.FloorToInt(dist / 100);
            Vector3 dir = targetControl.TargetPredictedPosition(predictionFrames) - transform.position;
            targetRot = Quaternion.LookRotation(dir.normalized, transform.up);
            cachedTransform.rotation = Quaternion.RotateTowards(cachedTransform.rotation, targetRot, rotateSpeed * Time.deltaTime);
        }
        else
        {
            cachedTransform.localRotation =
                Quaternion.RotateTowards(cachedTransform.localRotation, startRot, rotateSpeed * Time.deltaTime);
        }
        
        while (Quaternion.Angle(cachedTransform.localRotation, startRot) > maxAngle)
        {
            cachedTransform.localRotation =
                Quaternion.RotateTowards(cachedTransform.localRotation, startRot, rotateSpeed * Time.deltaTime);

        }
    }
}
