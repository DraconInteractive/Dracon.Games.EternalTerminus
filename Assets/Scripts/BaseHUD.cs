using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseHUD : MonoBehaviour
{
    public bool ctxMenuActive;
    public virtual void Activate()
    {
        
    }

    public virtual void Deactivate()
    {
        
    }
    
    public virtual void UpdateHUD()
    {

    }
    
    public void ToggleCtxMenu()
    {
        ToggleCtxMenu(!ctxMenuActive);
    }

    public virtual void ToggleCtxMenu(bool state)
    {
        ctxMenuActive = state;
    }
}
