using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UI_ContextMenu : MonoBehaviour
{
    public bool open;
    private CanvasGroup group;
    public GameObject ctxActionPrefab;
    
    private List<UI_ContextMenuItem> _menuItems = new List<UI_ContextMenuItem>();
    private int _currentItem;

    private bool openedThisFrame;
    private void Start()
    {
        group = GetComponent<CanvasGroup>();
    }

    public void Toggle()
    {
        if (open)
        {
            Close();
        }
        else
        {
            Open();
        }
    }
    
    public void Open()
    {
        openedThisFrame = true;
        
        var input = InputController.Instance;
        input.FromID("Menu Select").inputEvent += MenuSelectHandler;
        input.FromID("Menu Return").inputEvent += MenuReturnHandler;
        input.FromID("Menu Up").inputEvent += MenuUpHandler;
        input.FromID("Menu Down").inputEvent += MenuDownHandler;
        input.FromID("Menu Left").inputEvent += MenuLeftHandler;
        input.FromID("Menu Right").inputEvent += MenuRightHandler;
        
        // Clear old options
        foreach (Transform child in this.transform)
        {
            Destroy(child.gameObject);
        }
        _menuItems.Clear();
        
        BaseTargetable target = Player.Instance.currentShip.trackedTarget.Item1;
        if (target == null)
        {
            // Show only default options? 
            // None at the moment, so returning
            return;
        }

        var actions = target.GetContextActions().ToList();
        if (actions.Count == 0)
        {
            if (Player.Instance.currentShip.flightState == Ship.FlightState.Docked)
            {
                actions.Add(new ContextAction()
                {
                    ID = "Undock",
                    Action = () => { Player.Instance.currentShip.dockedAt.DockAction();}
                });
            }
            else
            {
                var menuItem_null = Instantiate(ctxActionPrefab, this.transform).GetComponent<UI_ContextMenuItem>();
                menuItem_null.text.text = "No Actions";
            }
            return;
        }
        
        foreach (var action in actions)
        {
            var menuItem = Instantiate(ctxActionPrefab, this.transform).GetComponent<UI_ContextMenuItem>();
            menuItem.text.text = action.ID;
            menuItem.onPress = action.Action;
            _menuItems.Add(menuItem);
        }

        _menuItems[0].AddHighlight();
        _currentItem = 0;
        
        group.alpha = 1;
        open = true;
        
        InputController.Instance.AssessContext();
    }

    public void Close()
    {
        var input = InputController.Instance;
        input.FromID("Menu Select").inputEvent -= MenuSelectHandler;
        input.FromID("Menu Return").inputEvent -= MenuReturnHandler;
        input.FromID("Menu Up").inputEvent -= MenuUpHandler;
        input.FromID("Menu Down").inputEvent -= MenuDownHandler;
        input.FromID("Menu Left").inputEvent -= MenuLeftHandler;
        input.FromID("Menu Right").inputEvent -= MenuRightHandler;
        
        group.alpha = 0;
        open = false;
        
        InputController.Instance.AssessContext();
    }
    
    void MenuSelectHandler(InputController.Context ctx, InputController.InputEventType evt)
    {
        if (evt == InputController.InputEventType.Down)
        {
            _menuItems[_currentItem].onPress?.Invoke();
            Close();
        }
    }
    
    void MenuReturnHandler(InputController.Context ctx, InputController.InputEventType evt)
    {
        if (evt == InputController.InputEventType.Down)
        {
            if (openedThisFrame)
            {
                openedThisFrame = false;
            }
            else
            {
                Close();
            }
        }
    }
    
    void MenuUpHandler(InputController.Context ctx, InputController.InputEventType evt)
    {
        if (evt != InputController.InputEventType.Down)
        {
            return;
        }
        if (_menuItems.Count == 0)
        {
            _currentItem = 0;
            return;
        }
        
        _menuItems[_currentItem].RemoveHighlight();
        
        _currentItem--;
        if (_currentItem < 0)
        {
            _currentItem = _menuItems.Count - 1;
        }
        
        _menuItems[_currentItem].AddHighlight();
    }
    
    void MenuDownHandler(InputController.Context ctx, InputController.InputEventType evt)
    {
        if (evt != InputController.InputEventType.Down)
        {
            return;
        }
        if (_menuItems.Count == 0)
        {
            _currentItem = 0;
            return;
        }
        
        _menuItems[_currentItem].RemoveHighlight();

        _currentItem++;
        if (_currentItem > _menuItems.Count - 1)
        {
            _currentItem = 0;
        }
        
        _menuItems[_currentItem].AddHighlight();
    }
    
    void MenuLeftHandler(InputController.Context ctx, InputController.InputEventType evt)
    {
        // tabbed filters in the future?   
    }
    
    void MenuRightHandler(InputController.Context ctx, InputController.InputEventType evt)
    {
        
    }
}
