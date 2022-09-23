using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public Camera mainCam;
    [SerializeField]
    private CameraPoint tpPoint;
    private List<CameraPoint> cameraPoints = new List<CameraPoint>();
    public int currentIndex;
    public CameraPoint CurrentPoint => cameraPoints[currentIndex];
    public BaseHUD CurrentHUD => CurrentPoint.HUD;

    private void Start()
    {
        mainCam = Camera.main;
        SetupInput();
    }

    void SetupInput()
    {
        InputController.Instance.FromID("Previous Camera").inputEvent += (x, y) =>
        {
            if (y == InputController.InputEventType.Down)
                PreviousPoint();
        };
        InputController.Instance.FromID("Next Camera").inputEvent += (x,y) => 
        {
            if (y == InputController.InputEventType.Down)
                NextPoint();
        };
    }

    private void FixedUpdate()
    {
        if (cameraPoints == null || cameraPoints.Count == 0)
        {
            return;
        }
        
        cameraPoints[currentIndex].TransformCamera(mainCam.transform);
        cameraPoints[currentIndex].HUD.UpdateHUD();
    }

    public void SetShip(Ship ship)
    {
        ship.onAttachComponent += CheckNewComponent;
        CheckNewComponent(null, null);
        SetPoint(0);
    }

    void CheckNewComponent(ShipComponentAnchor anchor, ShipComponent component)
    {
        Ship ship = Player.Instance.currentShip;
        tpPoint.transform.localPosition = ship.data.cameraOffset;
        cameraPoints.Clear();
        cameraPoints.Add(tpPoint);
        ShipComponentAnchor[] anchors = ship.Anchors.Where(x => x.component is CockpitComponent).ToArray();
        foreach (var cockpit in anchors)
        {
            cameraPoints.Add((cockpit.component as CockpitComponent).cameraPoint);
        }
    }

    public void SetPoint (int newPointIndex)
    {
        cameraPoints[currentIndex].SetInactive();
        currentIndex = newPointIndex;
        cameraPoints[currentIndex].SetActive();
    }

    public void NextPoint()
    {
        var i = currentIndex;
        i++;
        if (i >= cameraPoints.Count)
        {
            i = 0;
        }
        SetPoint(i);
    }

    public void PreviousPoint()
    {
        var i = currentIndex;
        i--;
        if (i < 0)
        {
            i = cameraPoints.Count - 1;
        }
        SetPoint(i);
    }
}
