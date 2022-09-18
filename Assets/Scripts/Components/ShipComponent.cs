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
    
    public virtual void OnAttached (ShipComponentAnchor anchor)
    {
        
    }

    public virtual void OnRemoved (ShipComponentAnchor anchor)
    {
        
    }
    
    
}
