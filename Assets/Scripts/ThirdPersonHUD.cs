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
    public Image predictiveCrosshair;
    private TargetController targeting;
    private CameraController cameraControl;

    public Color noTargetColor, targetAcquiredColor, targetLockedColor;
    
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
        predictiveCrosshair.gameObject.SetActive(false);
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
        
        if (!predictiveCrosshair.gameObject.activeSelf && state == TargetController.TargetState.TargetLocked)
        {
            predictiveCrosshair.gameObject.SetActive(true);
        } else if (state != TargetController.TargetState.TargetLocked && predictiveCrosshair.gameObject.activeSelf)
        {
            predictiveCrosshair.gameObject.SetActive(false);
        }

        Vector2 screenPos = Vector2.zero;
        Vector2 predictiveScreenPos = Vector2.zero;
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
                int predictionFrames = Mathf.FloorToInt(Vector3.Distance(transform.position, targeting.trackedTarget.Item1.Position()) / 100);
                predictiveScreenPos =
                    cameraControl.mainCam.WorldToViewportPoint(targeting.TargetPredictedPosition(1));
                crossHair.color = targetLockedColor;
                break;
        }
        crossHair.rectTransform.anchorMin = screenPos;
        crossHair.rectTransform.anchorMax = screenPos;
        predictiveCrosshair.rectTransform.anchorMin = predictiveScreenPos;
        predictiveCrosshair.rectTransform.anchorMax = predictiveScreenPos;
    }
}
