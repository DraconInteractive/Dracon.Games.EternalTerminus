using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FAction_Placeholder : FlowAction
{
    [TextArea]
    public string Note;

    public override void Begin()
    {
        base.Begin();

        Complete();
    }
}
