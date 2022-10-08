using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController_AI : BaseShipController
{
    public float decisionPeriod;
    private float _lastDecisionTime;

    public override void Initialize(Ship ship)
    {
        base.Initialize(ship);
    }

    public override void Deinitialize()
    {
        base.Deinitialize();
        Destroy(this);
    }

    private void FixedUpdate()
    {
        if (Time.time - _lastDecisionTime >= decisionPeriod)
        {
            _lastDecisionTime = Time.time;

            AIAction actionToExecute = UtilityActions.Instance.GetBestActionForShip(_ship);
            if (actionToExecute != null)
            {
                actionToExecute.Execute(_ship);
            }
        }
    }
}
