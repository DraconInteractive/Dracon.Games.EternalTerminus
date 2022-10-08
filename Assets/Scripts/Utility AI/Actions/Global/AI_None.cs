using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_None : AIAction
{
    public override bool CanExecute(Ship ship)
    {
        return true;
    }

    public override float GetScore(Ship ship)
    {
        return 0.1f;
    }

    public override void Execute(Ship ship)
    {
        
    }
}
