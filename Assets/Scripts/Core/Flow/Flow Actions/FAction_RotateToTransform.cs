using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class FAction_RotateToTransform : FlowAction
{
    public Transform target;
    public Transform rotateTo;

    [SuffixLabel("- use 0 to snap immediately")]
    public float duration;
    
    public override void Begin()
    {
        base.Begin();

        if (duration == 0)
        {
            target.rotation = rotateTo.rotation;
            Complete();
        }
        else
        {
            instigator.StartCoroutine(Run());
        }
    }

    IEnumerator Run ()
    {
        Quaternion startRotation = target.rotation;
        for (float f = 0; f < 1; f += Time.deltaTime / duration)
        {
            target.rotation = Quaternion.Lerp(startRotation, rotateTo.rotation, f);
            yield return null;
        }
        Complete();
    }
}
