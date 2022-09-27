using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class InputController : Manager<InputController>
{
    #region Definitions
    
    public enum Context
    {
        None,
        Flight,
        Menu,
        Docked
    }

    public enum InputEventType
    {
        Down,
        Up,
        Held
    }
    
    public delegate void OnInputEvent(Context ctx, InputEventType eventType);
    
    [System.Serializable]
    public class Input
    {
        public string ID;
        public InputAction action;
        public OnInputEvent inputEvent;
        public Context[] supportedContexts;
        private InputController controller;

        public void Setup(InputController _inputController)
        {
            controller = _inputController;
        }
        
        public void OnEnable()
        {
            action.started += InputHandler;
            action.canceled += InputHandler;
            action.Enable();
        }

        public void OnDisable()
        {
            action.started -= InputHandler;
            action.canceled -= InputHandler;
            action.Disable();
        }
        
        void InputHandler(InputAction.CallbackContext ctx)
        {
            var controllerContext = controller.currentContext;
            if (!supportedContexts.Contains(controllerContext))
            {
                return;
            }
            if (ctx.phase == InputActionPhase.Started)
            {
                inputEvent?.Invoke(controller.currentContext, InputEventType.Down);
            }
            else if (ctx.phase == InputActionPhase.Canceled)
            {
                inputEvent?.Invoke(controller.currentContext, InputEventType.Up);
            }
        }
    }

    #endregion

    public Input[] AllInputs;
    public Context currentContext;
    private void Awake()
    {
        foreach (var i in AllInputs)
        {
            i.Setup(this);
        }
    }

    private void OnEnable()
    {
        foreach (var i in AllInputs)
        {
            i.OnEnable();
        }
    }
    
    private void OnDisable()
    {
        foreach (var i in AllInputs)
        {
            i.OnDisable();
        }
    }

    public void AssessContext()
    {
        Ship ship = Player.Instance.currentShip;
        if (ship == null)
        {
            currentContext = Context.None;
            return;
        }
        bool docked = ship.flightState == Ship.FlightState.Docked;
        bool menuOpen = BaseHUD.activeHUD.contextMenu.open;

        if (menuOpen)
        {
            currentContext = Context.Menu;
        }
        else if (docked)
        {
            currentContext = Context.Docked;
        }
        else
        {
            currentContext = Context.Flight;
        }
    }
    
    public Input FromID(string _id)
    {
        return AllInputs.FirstOrDefault(x => x.ID == _id);
    }
}
