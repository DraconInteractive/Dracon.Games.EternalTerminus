using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FAction_FlowBranch : FlowAction
{
    public FlowCondition condition;
    public FlowHost[] flowHosts;

    public override void Begin()
    {
        base.Begin();
        // evaluates condition
        int i = condition.Evaluate();
        // runs selected flowhost
        flowHosts[i].Run();
        Complete();
    }
}
