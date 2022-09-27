using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(menuName = "Cortiical/Flow Actions/Set Animator Trigger")]
public class FAction_SetAnimatorTrigger : FlowAction
{
    public Animator target;
    public string triggerName;
    public override void Begin()
    {
        base.Begin();

        target.SetTrigger(triggerName);

        Complete();
    }
}
