using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FAction_TriggerFlowHost : FlowAction
{
    public FlowHost target;
    public override void Begin()
    {
        base.Begin();

        target.Run();

        Complete();
    }
}
