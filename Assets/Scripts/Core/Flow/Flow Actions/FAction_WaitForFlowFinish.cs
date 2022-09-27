using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FAction_WaitForFlowFinish : FlowAction
{
    public FlowHost targetHost;

    public override void Begin()
    {
        base.Begin();
        instigator.StartCoroutine(Run());
    }

    IEnumerator Run ()
    {
        while (targetHost.state != FlowHost.State.Complete)
        {
            yield return null;
        }
        Complete();
        yield break;
    }
}
