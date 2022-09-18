using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class Player : Manager<Player>
{
    public Ship currentShip;
    public FlightController flightController;
    public WeaponController weaponController;
    public TargetController TargetController;
    public CameraController cameraController;
    public InputController inputController;
    
    private void Awake()
    {
        ToggleCursor(false);
        inputController.FromID("Toggle Cursor").inputEvent += (x, y) => ToggleCursor();
        inputController.FromID("Context Interaction").inputEvent += (x, y) => ContextInteraction();
    }

    private void Start()
    {
        if (currentShip != null)
        {
            SetShip(currentShip);
        }
    }

    public void ToggleCursor()
    {
        ToggleCursor(!Cursor.visible);
    }

    public void ToggleCursor(bool state)
    {
        Cursor.visible = state;
        Cursor.lockState = state ? CursorLockMode.None : CursorLockMode.Locked;
        UpdateLog();
    }

    public void SetShip(Ship newShip)
    {
        currentShip = newShip;
        //Add remove ship to cleanup hooks
        flightController.SetShip(newShip);
        weaponController.SetShip(newShip);
        cameraController.SetShip(newShip);
        UpdateLog();
    }

    void UpdateLog()
    {
        string log = "<b>Player</b>\n";
        log += $"Ship: {(currentShip == null ? "NULL" : currentShip.data.displayName)} \n";
        log += $"Cursor Visible: {Cursor.visible}\n";
        log += $"Cursor Lock State: {Cursor.lockState}\n";
        DLog.Instance.AddOrUpdate(this, log);
    }

    void ContextInteraction()
    {
        cameraController.CurrentHUD.ToggleCtxMenu();
    }
}
