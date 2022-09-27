using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FCondition_RandomSelect : FlowCondition
{
    public int rangeMin,rangeMax;

    public override int Evaluate()
    {
        return Random.Range(rangeMin, rangeMax);
    }
}
