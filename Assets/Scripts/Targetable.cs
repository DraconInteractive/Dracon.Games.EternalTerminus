using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targetable : Pawn
{
    public static List<Targetable> All = new List<Targetable>();
    protected ContextAction[] actions;
    public Vector3 velocity;

    public virtual Vector3 Position()
    {
        return alive ? transform.position : Vector3.zero;
    }

    protected override void OnEnable()
    {
        RegisterTarget();
    }
    protected override void OnDisable()
    {
        RemoveTarget();
    }

    public virtual void RegisterTarget()
    {
        All.Add(this);
    }

    public virtual void RemoveTarget()
    {
        All.Remove(this);
    }

    public ContextAction[] GetContextActions()
    {
        return actions;
    }

    
}

[System.Serializable]
public class ContextAction
{
    public string ID;
    public Action Action;
}