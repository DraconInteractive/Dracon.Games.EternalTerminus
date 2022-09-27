using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class FAction_MoveToTransform : FlowAction
{
    public Transform target;
    public Transform moveTo;
    public bool matchRotation;

    [SuffixLabel("- use 0 to snap immediately")]
    public float duration;
    
    // Reset
    private Vector3 startPos;
    private Quaternion startRot;
    
    public override void Begin()
    {
        base.Begin();

        startPos = target.position;
        startRot = target.rotation;
        
        if (duration == 0)
        {
            target.position = moveTo.position;
            if (matchRotation)
            {
                target.rotation = moveTo.rotation;
            }
            Complete();
        }
        else
        {
            instigator.StartCoroutine(Run());
        }
    }

    IEnumerator Run ()
    {
        Transform startPosition = target;
        for (float f = 0; f < 1; f += Time.deltaTime / duration)
        {
            target.position = Vector3.Lerp(startPosition.position, moveTo.position, f);
            if (matchRotation)
            {
                target.rotation = Quaternion.Lerp(startPosition.rotation, moveTo.rotation, f);
            }
            yield return null;
        }
        Complete();
    }

    public override void Reset()
    {
        base.Reset();
        target.position = startPos;
        target.rotation = startRot;
    }
}
