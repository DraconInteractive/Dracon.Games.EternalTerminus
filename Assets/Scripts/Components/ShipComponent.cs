using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipComponent : MonoBehaviour
{
    public enum Tag
    {
        Weapon, 
        Armour,
        Shield, 
        Cockpit,
        Engine,
        Primary,
        Secondary,
        Tertiary,
        Docking
    }

    public Tag[] tags;

    protected ShipComponentAnchor _anchor;

    public virtual void OnAttached (ShipComponentAnchor anchor)
    {
        _anchor = anchor;
    }

    public virtual void OnRemoved (ShipComponentAnchor anchor)
    {
        _anchor = null;
    }
    
    
}
