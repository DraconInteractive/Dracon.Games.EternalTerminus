using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
public abstract class FlowAction
{
    public enum State
    {
        Idle,
        Running, 
        Complete,
        Cancelled
    }
    [ReadOnly, BoxGroup]
    public State state;
    [BoxGroup, GUIColor("SkipColor")]
    public bool skip;
    protected FlowHost instigator;
#if UNITY_EDITOR
    private Color SkipColor()
    {
        Sirenix.Utilities.Editor.GUIHelper.RequestRepaint();
        if (skip)
        {
            return Color.red;
        }
        else
        {
            return Color.white;
        }
    }
    #endif
    public virtual void Setup (FlowHost _instigator)
    {
        instigator = _instigator;
    }

    public virtual void Begin ()
    {
        state = State.Running;
    }

    public virtual void Complete ()
    {
        state = State.Complete;
    }

    public virtual void Reset ()
    {
        state = State.Idle;
    }

    public virtual void Cancel ()
    {
        state = State.Cancelled;
    }
}
