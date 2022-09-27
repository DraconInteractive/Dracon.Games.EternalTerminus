using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//[CreateAssetMenu(menuName = "Cortiical/Flow Actions/Toggle GO")]
public class FAction_ToggleGameObject : FlowAction
{
    public GameObject[] targets;
    public enum ToggleType
    {
        Toggle,
        TurnOn,
        TurnOff
    }
    public ToggleType type;

    public override void Begin()
    {
        base.Begin();
        foreach (var target in targets)
        {
            switch (type)
            {
                case ToggleType.Toggle:
                    target.SetActive(target.activeSelf);
                    break;
                case ToggleType.TurnOn:
                    target.SetActive(true);
                    break;
                case ToggleType.TurnOff:
                    target.SetActive(false);
                    break;
            }
        }

        Complete();
    }
}
