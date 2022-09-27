using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FAction_DebugLog : FlowAction
{
    public string output;
    public override void Begin()
    {
        base.Begin();

        Debug.Log(output);

        Complete();
    }
}
