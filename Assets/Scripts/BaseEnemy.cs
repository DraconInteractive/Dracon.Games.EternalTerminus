using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : MonoBehaviour, ITargetable
{
    // Make this into a scriptable object later (so its shared between different enemies)
    ContextAction[] actions;

    private void Start()
    {
        actions = new ContextAction[0];
    }

    protected void OnEnable()
    {
        Register();
    }

    protected void OnDisable()
    {
        Deregister();
    }

    #region Targeting
    public void Register()
    {
        TargetController.AllTargets.Add(this);
    }

    public void Deregister()
    {
        TargetController.AllTargets.Remove(this);
    }

    public Vector3 Position()
    {
        return transform.position;
    }

    public string ID()
    {
        return this.gameObject.name;
    }
    #endregion

    #region Interaction

    public ContextAction[] GetContextActions()
    {
        return actions;
    }

    #endregion
}
