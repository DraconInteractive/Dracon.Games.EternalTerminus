using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
public class FlightController : Manager<FlightController>
{
    private Player player;
    [Space]
    public InputAction throttleAction;
    public InputAction pitchAction;
    public InputAction rollAction;
    public InputAction yawAction;

    public void Setup()
    {
        player = Player.Instance;
    }
    
    // TODO: Move actions to InputController
    private void OnEnable()
    {
        throttleAction.Enable();
        pitchAction.Enable();
        rollAction.Enable();
        yawAction.Enable();
    }

    private void OnDisable()
    {
        throttleAction.Disable();
        pitchAction.Disable();
        rollAction.Disable();
        yawAction.Disable();
    }

    private void Update()
    {
        if (player.currentShip == null) return;
        
        player.currentShip.ThrottleIn = throttleAction.ReadValue<float>();
        player.currentShip.PitchIn = pitchAction.ReadValue<float>();
        player.currentShip.RollIn = rollAction.ReadValue<float>();
        player.currentShip.YawIn = yawAction.ReadValue<float>();
    }
}
