using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dock : Targetable 
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

    protected override void Start()
    {
        actions = new []
        {
            new ContextAction()
            {
                ID = "Dock / Undock",
                Action = () => DockAction()
            },
            new ContextAction()
            {
                ID = "Set Hostile",
                Action = () => Debug.Log("Fake Action")
            },
            new ContextAction()
            {
                ID = "Begin Comms",
                Action = () => Debug.Log("Fake Action")
            }
        };
    }

    public void DockAction()
    {
        Ship ship = Player.Instance.currentShip;
        if (ship.flightState == Ship.FlightState.InFlight)
        {
            ship.StartDocking(this);
        } 
        else if (ship.flightState == Ship.FlightState.Docked)
        {
            ship.StartUndocking();
        }
    }

    public void DockingComplete(Ship ship)
    {
        ship.dockedAt = this;
    }
}