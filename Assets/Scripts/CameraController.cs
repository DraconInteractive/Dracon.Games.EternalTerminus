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
    
    [Space]
    public float movementSpeed;
    public float rotationSpeed;

    private void Start()
    {
        SetupInput();
    }

    void SetupInput()
    {
        InputController.Instance.FromID("Previous Camera").inputEvent += (x,y) => PreviousPoint();
        InputController.Instance.FromID("Next Camera").inputEvent += (x,y) => NextPoint();
    }

    private void Update()
    {
        if (cameraPoints == null || cameraPoints.Count == 0)
        {
            return;
        }
        
        Vector3 targetPos = cameraPoints[currentIndex].transform.position;
        Quaternion targetRot = cameraPoints[currentIndex].transform.rotation;
        
        mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, targetPos, movementSpeed * Time.deltaTime);
        mainCam.transform.rotation = Quaternion.Lerp(mainCam.transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        
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
