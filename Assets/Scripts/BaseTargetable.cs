using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseTargetable : MonoBehaviour
{
    public static List<BaseTargetable> All = new List<BaseTargetable>();
    protected ContextAction[] actions;
    public Vector3 velocity;
    public virtual string ID()
    {
        return this.gameObject.name;
    }
    
    public virtual Vector3 Position()
    {
        return transform.position;
    }

    protected virtual void OnEnable()
    {
        Register();
    }
    protected virtual void OnDisable()
    {
        Deregister();
    }

    public virtual void Register()
    {
        All.Add(this);
    }

    public virtual void Deregister()
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