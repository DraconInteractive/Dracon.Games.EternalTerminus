using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DraconTools
{
    public static Vector3 TargetPredictedPosition(BaseTargetable target, float t)
    {
        if (target == null)
        {
            return Vector3.zero;
        }

        //final pos = initial pos + speed * t + 0.5 * accel * t ^2
        return target.Position() + target.velocity * t;
    }
    
    public static Vector3 TargetPredictedPosition(BaseTargetable target, float projectileSpeed, float targetDistance)
    {
        if (target == null)
        {
            return Vector3.zero;
        }

        // TODO: Fix this?
        float t = targetDistance / projectileSpeed;
        //final pos = initial pos + speed * t + 0.5 * accel * t ^2
        return target.Position() + target.velocity * t;
    }
}
