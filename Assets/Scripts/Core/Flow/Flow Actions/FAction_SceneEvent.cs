using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FAction_SceneEvent : FlowAction
{
    public UnityEvent sceneEvent;

    public override void Begin()
    {
        base.Begin();
        sceneEvent?.Invoke();
        Complete();
    }

}
