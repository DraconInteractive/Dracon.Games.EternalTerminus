using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShipController_Player : BaseShipController
{
    private Player _player;
    
    private InputAction throttleAction;
    private InputAction pitchAction;
    private InputAction rollAction;
    private InputAction yawAction;

    private Coroutine primaryFireRoutine, secondaryFireRoutine;
    
    private void Update()
    {
        _ship.ThrottleIn = throttleAction.ReadValue<float>() * -1;
        _ship.PitchIn = pitchAction.ReadValue<float>();
        _ship.RollIn = rollAction.ReadValue<float>();
        _ship.YawIn = yawAction.ReadValue<float>();
    }

    public override void Initialize(Ship ship, params object[] data)
    {
        base.Initialize(ship);
        _player = Player.Instance;
        SetupInput();
    }

    public override void Deinitialize()
    {
        base.Deinitialize();
        
        InputController inputController = InputController.Instance;
        
        inputController.FromID("Toggle Target Tracking").inputEvent -= ToggleTargetTracking;
        inputController.FromID("Target Forward").inputEvent -= TargetForwardHandler;
        inputController.FromID("Target Closest").inputEvent -= ClosestTargetHandler;
        
        inputController.FromID("Primary Fire").inputEvent -= PrimaryFireHandler;
        inputController.FromID("Secondary Fire").inputEvent -= SecondaryFireHandler;
    }

    public void SetupInput()
    {
        InputController inputController = InputController.Instance;
        
        inputController.FromID("Toggle Target Tracking").inputEvent += ToggleTargetTracking;
        inputController.FromID("Target Forward").inputEvent += TargetForwardHandler;
        inputController.FromID("Target Closest").inputEvent += ClosestTargetHandler;
        
        inputController.FromID("Primary Fire").inputEvent += PrimaryFireHandler;
        inputController.FromID("Secondary Fire").inputEvent += SecondaryFireHandler;

        throttleAction = inputController.FromID("Throttle").action;
        pitchAction = inputController.FromID("Pitch").action;
        rollAction = inputController.FromID("Roll").action;
        yawAction = inputController.FromID("Yaw").action;
    }
    
    void ToggleTargetTracking(InputController.Context ctx, InputController.InputEventType evt)
    {
        if (evt == InputController.InputEventType.Down)
        {
            _player.currentShip.ToggleTargetTracking();
        }
    }

    void TargetForwardHandler(InputController.Context ctx, InputController.InputEventType evt)
    {
        if (evt == InputController.InputEventType.Down)
        {
            _player.currentShip.FindTarget(Ship.FindTargetMethod.Forward);
        }
    }

    void ClosestTargetHandler(InputController.Context ctx, InputController.InputEventType evt)
    {
        if (evt == InputController.InputEventType.Down)
        {
            _player.currentShip.FindTarget(Ship.FindTargetMethod.Closest);
        }
    }
    
    void PrimaryFireHandler(InputController.Context ctx, InputController.InputEventType evt)
    {
        if (evt == InputController.InputEventType.Down)
        {
            primaryFireRoutine = StartCoroutine(FireRoutine(0));
        }
        else if (evt == InputController.InputEventType.Up)
        {
            StopCoroutine(primaryFireRoutine);
        }
    }

    void SecondaryFireHandler(InputController.Context ctx, InputController.InputEventType evt)
    {
        if (evt == InputController.InputEventType.Down)
        {
            secondaryFireRoutine = StartCoroutine(FireRoutine(1));
        }
        else if (evt == InputController.InputEventType.Up)
        {
            StopCoroutine(secondaryFireRoutine);
        }
    }
    
    IEnumerator FireRoutine(int weaponGroup)
    {
        while (true)
        {
            switch (weaponGroup)
            {
                case 0:
                    _player.currentShip.FirePrimary();
                    break;
                case 1:
                    _player.currentShip.FireSecondary();
                    break;
            }

            yield return null;
        }
    }
}
