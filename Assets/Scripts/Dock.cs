using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dock : MonoBehaviour, ITargetable
{
    public Transform dockingPoint;
    public ContextAction[] actions;
    
    public Vector3 Position()
    {
        return dockingPoint.position;
    }

    public string ID()
    {
        return "Dock";
    }

    private void Start()
    {
        actions = new ContextAction[1]
        {
            new ContextAction()
            {
                ID = "Dock / Undock",
                Action = () => DockAction()
            }
        };
    }

    private void OnEnable()
    {
        Register();
    }

    private void OnDisable()
    {
        Deregister();
    }

    public void Register()
    {
        TargetController.AllTargets.Add(this);
    }

    public void Deregister()
    {
        TargetController.AllTargets.Remove(this);
    }

    public ContextAction[] GetContextActions()
    {
        return actions;
    }

    public void DockAction()
    {
        FlightController pFlight = Player.Instance.flightController;
        if (pFlight.state == FlightController.State.InFlight)
        {
            pFlight.StartDocking(this);
        } 
        else if (pFlight.state == FlightController.State.Docked)
        {
            // Undock
        }
    }

    public void DockingComplete(Ship ship) 
    {
        
    }
}
