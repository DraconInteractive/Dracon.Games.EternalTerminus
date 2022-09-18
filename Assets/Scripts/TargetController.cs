using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class TargetController : Manager<TargetController>
{
    public static List<ITargetable> AllTargets = new List<ITargetable>();

    public enum TargetState
    {
        TrackingInactive,
        NoTarget,
        TargetAcquired,
        TargetLocked
    };
    // Add scan controller and scanner component. 
    // scanner controller controls all target listings and aquisition
    // this is temp
    public float scanRange;
    
    public static bool trackingTargets;
    public (ITargetable, float) trackedTarget;
    public TargetState state;
    private WeaponController weapons;
    
    private void Start()
    {
        SetupInput();
        weapons = Player.Instance.weaponController;
    }

    private void Update()
    {
        UpdateCrosshair();
        UpdateLog();
    }

    void SetupInput()
    {
        var input = InputController.Instance;
        input.FromID("Toggle Target Tracking").inputEvent += ToggleTargetTracking;
        input.FromID("Target Forward").inputEvent += TargetForwardHandler;
        input.FromID("Target Closest").inputEvent += ClosestTargetHandler;
    }
    
    void ToggleTargetTracking(InputController.Context ctx, InputController.InputEventType evt)
    {
        if (evt == InputController.InputEventType.Down)
        {
            ToggleTargetTracking();
        }
    }

    void TargetForwardHandler(InputController.Context ctx, InputController.InputEventType evt)
    {
        if (evt == InputController.InputEventType.Down)
        {
            GetAimedTarget();
        }
    }

    void ClosestTargetHandler(InputController.Context ctx, InputController.InputEventType evt)
    {
        if (evt == InputController.InputEventType.Down)
        {
            GetClosestTarget();
        }
    }
    
    void UpdateCrosshair()
    {
        if (!trackingTargets)
        {
            state = TargetState.TrackingInactive;
            return;
        }
        
        if (trackedTarget.Item1 != null)
        {
            Vector3 targetDir = trackedTarget.Item1.Position() - transform.position;
            float dot = Vector3.Dot(transform.forward, targetDir.normalized);
            float currentTrackingLevel = trackedTarget.Item2;
            if (dot > weapons.MinWeaponDot() && Vector3.Distance(transform.position, trackedTarget.Item1.Position()) < weapons.MaxWeaponRange())
            {
                currentTrackingLevel += Time.deltaTime;
            }
            else
            {
                currentTrackingLevel -= Time.deltaTime;
            }
            trackedTarget.Item2 = Mathf.Clamp01(currentTrackingLevel);

            if (currentTrackingLevel >= 1)
            {
                state = TargetState.TargetLocked;
            }
            else
            {
                state = TargetState.TargetAcquired;
            }
        }
        else
        {
            state = TargetState.NoTarget;
        }
    }
    
    void GetAimedTarget()
    {
        if (!trackingTargets)
        {
            trackedTarget = (null,0);
            return;
        }
        ITargetable tempTarget = null;
        
        float minDot = 0;
        var viableTargets = AllTargets
            .Where(x => Vector3.Distance(x.Position(), transform.position) < scanRange);

        foreach (var target in viableTargets)
        {
            Vector3 dir = target.Position() - transform.position;
            float dot = Vector3.Dot(transform.forward, dir.normalized);
            Debug.Log(target.ID() + ": " + dot);
            if (dot > minDot)
            {
                minDot = dot;
                tempTarget = target;
            }
        }
        trackedTarget = (tempTarget, 0);
    }

    void GetClosestTarget()
    {
        if (!trackingTargets)
        {
            trackedTarget = (null,0);
            return;
        }
        ITargetable tempTarget = null;
        float maxRange = scanRange;
        Vector3 pos = transform.position;
        foreach (var target in AllTargets)
        {
            float dist = Vector3.Distance(target.Position(), pos);
            if (dist < maxRange)
            {
                maxRange = dist;
                tempTarget = target;
            }
        }
        trackedTarget = (tempTarget, 0);
    }

    void ToggleTargetTracking()
    {
        trackingTargets = !trackingTargets;
        trackedTarget = (null, 0);
    }

    void UpdateLog()
    {
        string log = "<b>Tracking</b>\n";
        log += $"Tracking targets: {trackingTargets}\n";
        log += $"Target: {(trackedTarget.Item1 != null ? trackedTarget.Item1.ID() : "NULL")}";
        DLog.Instance.AddOrUpdate(this, log);
    }
    
    #if UNITY_EDITOR
    [ContextMenu("Output All Targets")]
    public void DEBUG_AllTargets()
    {
        foreach (var target in AllTargets)
        {
            Debug.Log(target.ID());
        }
    }
    #endif
}
