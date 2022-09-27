using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FAction_Move_Offset : FlowAction
{
    public enum Scope
    {
        Local,
        Global
    }

    public Scope scope;
    public Transform target;
    public Vector3 offset;
    public override void Begin()
    {
        base.Begin();
        switch (scope)
        {
            case Scope.Local:
                target.localPosition += offset;
                break;
            case Scope.Global:
                target.position += offset;
                break;
        }
        Complete();
    }

    public override void Reset()
    {
        base.Reset();

        switch (scope)
        {
            case Scope.Local:
                target.localPosition -= offset;
                break;
            case Scope.Global:
                target.position -= offset;
                break;
        }
    }
}
