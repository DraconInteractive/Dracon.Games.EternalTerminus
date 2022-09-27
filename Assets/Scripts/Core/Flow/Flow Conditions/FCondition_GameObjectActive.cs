using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FCondition_GameObjectActive : FlowCondition
{
    public GameObject target;

    public override int Evaluate()
    {
        return (target.activeSelf) ? 1 : 0;
    }
}
