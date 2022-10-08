using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_PrimaryFire : AIAction_Interaction
{
    public override bool CanExecute(Ship ship)
    {
        return true;
    }

    public override float GetScore(Ship ship)
    {
        // adjust score for primary weapon cooldowns / how many are ready to fire
        return 0;
    }

    public override void Execute(Ship ship)
    {
        ship.FirePrimary();
    }
}
