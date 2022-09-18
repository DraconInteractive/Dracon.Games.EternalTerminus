using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class JoystickInput : MonoBehaviour
{
    [HideInInspector]
    public Joystick stick;

    public delegate void OnTriggerPress();

    public OnTriggerPress onTriggerPress;
    
    // Update is called once per frame
    void Update()
    {
        if (stick == null)
        {
            FindStick();
            return;
        }

        if (stick.trigger.wasPressedThisFrame)
        {
            onTriggerPress?.Invoke();
        }
    }

    void FindStick()
    {
        stick = Joystick.current;
    }

    public Vector3 Main()
    {
        return new Vector3(stick.stick.x.ReadValue(), stick.stick.y.ReadValue(), stick.twist.ReadValue());
    }
}
