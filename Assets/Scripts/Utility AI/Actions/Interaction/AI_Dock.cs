using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Dock : AIAction_Interaction
{
    public override bool CanExecute(Ship ship)
    {
        // return false if docked, in combat
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
        // if tracked target is not a dock, find a good dock (or do this in scoring)
    }
}
