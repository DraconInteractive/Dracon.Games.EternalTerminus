using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AI_FlyRandom : AIAction_Locomotion
{
    public override bool CanExecute(Ship ship)
    {
        if (ship.flightState != Ship.FlightState.InFlight)
        {
            return false;
        }
        return true;
    }

    public override float GetScore(Ship ship)
    {
        return 0;
    }

    public override void Execute(Ship ship)
    {
        var range = 100;
        Vector3 target = Random.insideUnitSphere * range;
        // fly to target
        // make target part of blackboard
    }
}
