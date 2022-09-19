using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dock : BaseTargetable 
{
    public Transform dockingPoint;
    
    public override Vector3 Position()
    {
        return dockingPoint.position;
    }

    public override string ID()
    {
        return "Dock";
    }

    private void Start()
    {
        actions = new []
        {
            new ContextAction()
            {
                ID = "Dock / Undock",
                Action = () => DockAction()
            }
        };
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
        ship.dockedAt = this;
    }
}