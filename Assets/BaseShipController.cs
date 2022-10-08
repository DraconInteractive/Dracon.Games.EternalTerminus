using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseShipController : MonoBehaviour
{
    protected Ship _ship;

    public virtual void Initialize(Ship ship, params object[] data)
    {
        _ship = ship;
    }

    public virtual void Deinitialize()
    {
        
    }
}
