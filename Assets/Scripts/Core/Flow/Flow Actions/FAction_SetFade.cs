using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FAction_SetFade : FlowAction
{
    public float target;
    public bool waitForFinish = true;

    public override void Begin()
    {
        base.Begin();

        instigator.StartCoroutine(Run());
    }

    IEnumerator Run()
    {
        GlobalFade.Instance.SetTarget(target);

        if (waitForFinish)
        {
            yield return new WaitForSeconds(GlobalFade.Instance.speed);
        }

        Complete();
        yield break;
    }
}
