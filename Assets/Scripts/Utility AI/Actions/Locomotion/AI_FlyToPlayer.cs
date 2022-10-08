using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_FlyToPlayer : AIAction_Locomotion
{
    public override bool CanExecute(Ship ship)
    {
        if (ship.dockedAt != null)
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
        
    }
}
