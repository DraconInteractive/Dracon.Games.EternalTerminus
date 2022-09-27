using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FAction_WaitForFrame : FlowAction
{
    public override void Begin()
    {
        base.Begin();
        instigator.StartCoroutine(Run());
    }

    IEnumerator Run ()
    {
        yield return null;
        Complete();
    }
}
