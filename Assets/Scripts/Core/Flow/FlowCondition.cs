using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class FlowCondition
{
    FlowHost instigator;
    public virtual void Setup (FlowHost _instigator)
    {
        instigator = _instigator;
    }

    public virtual int Evaluate ()
    {
        return 1;
    }

    public bool EvaluateAsBool ()
    {
        int i = Evaluate();
        return i > 0;
    }
}
