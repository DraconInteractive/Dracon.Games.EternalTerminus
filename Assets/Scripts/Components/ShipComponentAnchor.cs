using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShipComponentAnchor : MonoBehaviour
{
    public ShipComponent component;
    public ShipComponent.Tag[] compatibleWith;

    [HideInInspector] public Ship ship;
    
    public delegate void OnComponentAttached(ShipComponent component);
    public OnComponentAttached onComponentAttached;
    
    public delegate void OnComponentRemoved(ShipComponent component);
    public OnComponentRemoved onComponentRemoved;
    
    public T GetComponentAs<T>() where T : ShipComponent
    {
        return (T)component;
    }

    public void AttachComponent (ShipComponent newComponent)
    {
        // Check to see if any of the newComponents tags exist within the compatibility array
        if (!compatibleWith.Intersect(newComponent.tags).Any())
        {
            // If not, its not compatible, and cancel
            return;
        }
        
        RemoveComponent();

        component = newComponent;
        
        component.OnAttached(this);
        onComponentAttached?.Invoke(component);
    }

    public void RemoveComponent()
    {
        if (component != null)
        {
            component.OnRemoved(this);
            onComponentRemoved?.Invoke(component);
        }
    }

    private void OnDrawGizmos()
    {
        if (component == null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }
    }
}
