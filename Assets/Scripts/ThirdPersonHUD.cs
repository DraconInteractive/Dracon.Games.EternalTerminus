using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ThirdPersonHUD : BaseHUD
{
    [Header("Third Person")]
    public Canvas canvas;
    public Image crossHair;
    private TargetController targeting;
    private CameraController cameraControl;

    public Color noTargetColor, targetAcquiredColor, targetLockedColor;
    
    [Header("Context Menu")]
    public CanvasGroup ctxMenuGroup;
    public Transform ctxMenuContainer;
    public GameObject ctxActionPrefab;
    
    private bool showCrosshair;
    
    
    public override void Activate()
    {
        base.Activate();
        cameraControl = Player.Instance.cameraController;
        targeting = Player.Instance.TargetController;
        canvas.gameObject.SetActive(true);
    }

    public override void Deactivate()
    {
        base.Deactivate();
        canvas.gameObject.SetActive(false);
        crossHair.gameObject.SetActive(false);
    }

    public override void UpdateHUD()
    {
        base.UpdateHUD();

        var state = targeting.state;
        var currentTarget = targeting.trackedTarget.Item1;
        
        if (state == TargetController.TargetState.TrackingInactive)
        {
            showCrosshair = false;
        }
        else
        {
            showCrosshair = true;
        }

        if (showCrosshair && !crossHair.gameObject.activeSelf)
        {
            crossHair.gameObject.SetActive(true);
        }
        else if (!showCrosshair && crossHair.gameObject.activeSelf)
        {
            crossHair.gameObject.SetActive(false);
        }

        Vector2 screenPos = Vector2.zero;
        switch (state)
        {
            case TargetController.TargetState.NoTarget:
                Transform camT = cameraControl.mainCam.transform;
                screenPos = cameraControl.mainCam.WorldToViewportPoint(camT.position + camT.forward * 10);
                crossHair.color = noTargetColor;
                break;
            case TargetController.TargetState.TargetAcquired:
                screenPos = cameraControl.mainCam.WorldToViewportPoint(currentTarget.Position());
                crossHair.color = targetAcquiredColor;
                break;
            case TargetController.TargetState.TargetLocked:
                screenPos = cameraControl.mainCam.WorldToViewportPoint(currentTarget.Position());
                crossHair.color = targetLockedColor;
                break;
        }
        crossHair.rectTransform.anchorMin = screenPos;
        crossHair.rectTransform.anchorMax = screenPos;
        
    }

    public override void ToggleCtxMenu(bool state)
    {
        base.ToggleCtxMenu(state);
        ctxMenuGroup.alpha = state ? 1 : 0;
        if (state)
        {
            PopulateMenu();
        }
    }

    void PopulateMenu()
    {
        foreach (Transform child in ctxMenuContainer)
        {
            Destroy(child.gameObject);
        }

        BaseTargetable target = TargetController.Instance.trackedTarget.Item1;
        if (target == null)
        {
            // Show only default options? 
            // None at the moment, so returning
            return;
        }

        var actions = target.GetContextActions().ToList();
        if (actions.Count == 0)
        {
            if (Player.Instance.flightController.state == FlightController.State.Docked)
            {
                actions.Add(new ContextAction()
                {
                    ID = "Undock",
                    Action = () => { Player.Instance.currentShip.dockedAt.DockAction();}
                });
            }
            var actionUI = Instantiate(ctxActionPrefab, ctxMenuContainer).GetComponent<UI_ContextMenuItem>();
            actionUI.text.text = "No Actions";
            return;
        }
        
        foreach (var action in actions)
        {
            var actionUI = Instantiate(ctxActionPrefab, ctxMenuContainer).GetComponent<UI_ContextMenuItem>();
            actionUI.text.text = action.ID;
            actionUI.onPress = action.Action;
        }
        
    }
}
