using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseHUD : MonoBehaviour
{
    public static BaseHUD activeHUD;
    
    public UI_ContextMenu contextMenu;
    
    public virtual void Activate()
    {
        activeHUD = this;
    }

    public virtual void Deactivate()
    {
        
    }
    
    public virtual void UpdateHUD()
    {

    }
    
    public void ToggleCtxMenu()
    {
        contextMenu.Toggle();
        
        InputController.Instance.AssessContext();
    }

    public virtual void ToggleCtxMenu(bool state)
    {
        if (state)
        {
            contextMenu.Open();
        }
        else
        {
            contextMenu.Close();
        }
        
        InputController.Instance.AssessContext();
    }
}
