using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AI_FlyToEnemy : AIAction_Locomotion
{
    public override bool CanExecute(Ship ship)
    {
        if (ship.dockedAt != null || Targetable.All.Where(x => x is Enemy).Count() == 0)
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
