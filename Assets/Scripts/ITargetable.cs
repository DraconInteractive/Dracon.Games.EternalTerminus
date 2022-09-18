using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITargetable
{
    public Vector3 Position ();
    public string ID();
    
    public void Register();
    public void Deregister();

    public ContextAction[] GetContextActions();
}

[System.Serializable]
public class ContextAction
{
    public string ID;
    public Action Action;
}
    