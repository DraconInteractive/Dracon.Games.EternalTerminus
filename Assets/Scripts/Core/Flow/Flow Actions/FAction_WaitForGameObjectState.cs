using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FAction_WaitForGameObjectState : FlowAction
{
    public GameObject gameObject;
    public bool targetState;

    public override void Begin()
    {
        base.Begin();
        instigator.StartCoroutine(Run());
    }

    IEnumerator Run()
    {
        while (gameObject.activeSelf != targetState)
        {
            yield return null;
        }

        Complete();
        yield break;
    }
}
