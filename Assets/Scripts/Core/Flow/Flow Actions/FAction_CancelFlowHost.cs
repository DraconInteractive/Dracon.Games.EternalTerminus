using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FAction_CancelFlowHost : FlowAction
{
    public FlowHost target;
    public override void Begin()
    {
        base.Begin();

        target.Stop();

        Complete();
    }
}
