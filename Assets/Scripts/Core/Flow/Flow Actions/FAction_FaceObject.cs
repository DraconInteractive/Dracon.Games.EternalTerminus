using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FAction_FaceObject : FlowAction
{
    public Transform target;
    public Transform objectToFace;

    [SuffixLabel("- use 0 to snap immediately")]
    public float duration;

    //Reset
    private Quaternion startRotation;
    private Coroutine runRoutine;
    
    public override void Begin()
    {
        base.Begin();

        startRotation = target.rotation;
        
        if (duration == 0)
        {
            Vector3 dir = objectToFace.position - target.position;
            target.rotation = Quaternion.LookRotation(dir.normalized, Vector3.up);
            Complete();
        }
        else
        {
            runRoutine = instigator.StartCoroutine(Run());
        }
    }

    IEnumerator Run ()
    {
        Quaternion startRotation = target.rotation;
        for (float f = 0; f < 1; f += Time.deltaTime / duration)
        {
            Vector3 dir = objectToFace.position - target.position;
            Quaternion endRotation = Quaternion.LookRotation(dir.normalized, Vector3.up);
            target.rotation = Quaternion.Lerp (startRotation, endRotation, f);
            yield return null;
        }
        Complete();
    }

    public override void Reset()
    {
        base.Reset();
        if (runRoutine != null)
        {
            instigator.StopCoroutine(runRoutine);
        }

        target.rotation = startRotation;
    }
}
