using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FAction_WaitForSeconds : FlowAction
{
    public float secondsToWait;

    public override void Begin()
    {
        base.Begin();
        instigator.StartCoroutine(Run());
    }

    IEnumerator Run ()
    {
        for (float f = 0; f < 1; f += Time.deltaTime / secondsToWait)
        {
            yield return null;
            if (state == State.Cancelled)
            {
                yield break;
            }
        }
        Complete();
    }
}
