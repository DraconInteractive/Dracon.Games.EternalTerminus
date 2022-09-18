using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
public class FlightController : MonoBehaviour
{
    public enum State
    {
        Fixed,
        Floating,
        Docking,
        Docked,
        InFlight
    }

    public State state;

    #region Flight Variables

    private float baseAcceleration;
    private float accelerationModifier;
    public float Acceleration
    {
        get
        {
            return baseAcceleration + accelerationModifier;
        }
    }
    private float baseMaxSpeed;
    private float maxSpeedModifier;
    public float MaxSpeed
    {
        get
        {
            return baseMaxSpeed + maxSpeedModifier;
        }
    }
    public float Throttle
    {
        get
        {
            float throttle = throttleAction.ReadValue<float>() * -1 * 0.5f;
            throttle += 0.5f;
            return throttle;
        }
    }
    private float throttleDeadzone;
    public float TargetSpeed
    {
        get
        {
            float tSpeed = Mathf.Lerp(-MaxSpeed * 0.25f, MaxSpeed, Throttle);
            // Deadzone
            if (Mathf.Abs(tSpeed) < throttleDeadzone)
            {
                tSpeed = 0;
            }

            return tSpeed;
        }
    }
    
    private float yawSpeed;
    private float rollSpeed;
    private float pitchSpeed;

    #endregion

    [Space] 
    public bool debugAdvanced;
    
    [Space]
    public InputAction throttleAction;
    public InputAction pitchAction;
    public InputAction rollAction;
    public InputAction yawAction;
    
    private float currentSpeed;
    private Coroutine dockingRoutine;
    private Transform cachedTransform;
    private ShipComponent dockingComponent;
    
    
    public float Speed
    {
        get
        {
            return currentSpeed;
        }
    }

    private void Start()
    {
        cachedTransform = transform;
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

    // Update is called once per frame
    private void Update()
    {
        switch (state)
        {
            case State.InFlight:
                InFlightTick();
                break;
            case State.Docked:
                currentSpeed = 0;
                break;
            case State.Docking:
                // Speed handled by docking routing
                break;
            case State.Fixed:
                currentSpeed = 0;
                break;
            case State.Floating:
                currentSpeed = Mathf.MoveTowards(currentSpeed, 0, baseAcceleration * Time.deltaTime);
                break;
        }

        UpdateLog();
    }

    void InFlightTick()
    {
        currentSpeed = Mathf.MoveTowards(currentSpeed, TargetSpeed, Acceleration * Time.deltaTime);
        
        transform.position += transform.forward * (currentSpeed * Time.deltaTime);

        Vector3 rotationSolver = new Vector3();

        float speedRotationMultiplier = Remap(Mathf.Abs(currentSpeed), 0, MaxSpeed, 0f, 0.8f);
        Vector3 rotDelta = new Vector3()
        {
            x = pitchAction.ReadValue<float>() * pitchSpeed,
            y = yawAction.ReadValue<float>() * yawSpeed,
            z = rollAction.ReadValue<float>() * rollSpeed
        };
        rotDelta *= (1 - speedRotationMultiplier) * Time.deltaTime;
        rotationSolver.x += rotDelta.x;
        rotationSolver.y += rotDelta.y;
        rotationSolver.z += rotDelta.z;
        transform.localRotation *= Quaternion.Euler(rotationSolver);
    }
    
    public void SetShip (Ship newShip)
    {
        newShip.onAttachComponent += CheckNewComponent;
        if (newShip.data != null)
        {
            baseAcceleration = newShip.data.acceleration;
            baseMaxSpeed = newShip.data.maxSpeed;
            yawSpeed = newShip.data.yawSpeed;
            pitchSpeed = newShip.data.pitchSpeed;
            rollSpeed = newShip.data.rollSpeed;
            throttleDeadzone = newShip.data.throttleDeadzone;
        }
        else
        {
            baseAcceleration = 0;
            baseMaxSpeed = 0;
            yawSpeed = 0;
            pitchSpeed = 0;
            rollSpeed = 0;
            throttleDeadzone = 0;
        }
        
        CheckNewComponent(null, null);
    }

    public void SetState(State newState)
    {
        if (newState != State.Docking && dockingRoutine != null)
        {
            StopCoroutine(dockingRoutine);
        }

        state = newState;
    }
    
    void CheckNewComponent(ShipComponentAnchor anchor, ShipComponent component)
    {
        if (component is EngineComponent || component == null)
        {
            var engines = Player.Instance.currentShip.GetComponentsOfType<EngineComponent>();
            if (engines == null || engines.Count == 0)
            {
                accelerationModifier = 0;
                maxSpeedModifier = 0;
                UpdateLog();
                return;
            }
            
            accelerationModifier = 0;
            maxSpeedModifier = 0;
            foreach (var engine in engines)
            {
                accelerationModifier += engine.acceleration;
                maxSpeedModifier += engine.maxSpeed;
            }
        }
        else if (component.tags.Contains(ShipComponent.Tag.Docking))
        {
            dockingComponent = component;
        }
        UpdateLog();
    }

    void UpdateLog()
    {
        string log = "<b>Flight Controller</b>\n";
        log += $"State: {state}\n";
        if (debugAdvanced)
        {
            log += $"Accel: {baseAcceleration} + {accelerationModifier} = {Acceleration}\n";
            log += $"Raw Throttle: {throttleAction.ReadValue<float>()}\n";
            log += $"Throttle: {Throttle}\n";
            log += $"Max Speed: {baseMaxSpeed} + {maxSpeedModifier} = {MaxSpeed}\n\n";
            log += $"Current/Target Speed: {currentSpeed} / {TargetSpeed}\n\n";
            log += $"Pitch Speed: {pitchSpeed}\n";
            log += $"Yaw Speed: {yawSpeed}\n";
            log += $"Roll Speed: {rollSpeed}";
        }
        else
        {
            log += $"Current/Target Speed: {currentSpeed} / {TargetSpeed}";
        }

        DLog.Instance.AddOrUpdate(this, log);
    }

    public void StartDocking (Dock dock)
    {
        SetState(State.Docking);
        if (dockingRoutine != null) { StopCoroutine(dockingRoutine); }
        dockingRoutine = StartCoroutine(DockingRoutine(dock));
    }

    // TODO properly apply offset from docking component
    // Currently assumes docking component faces same direction as ship
    IEnumerator DockingRoutine(Dock dock)
    {
        Transform target = dock.dockingPoint;
        if (dockingComponent == null)
        {
            Debug.LogWarning("Docking Cancelled: Component Missing");
            CancelDocking();
        }

        Transform docker = dockingComponent.transform;

        // setup useful variables
        float rotSpeed = Mathf.Abs(pitchSpeed) + Mathf.Abs(yawSpeed) + Mathf.Abs(rollSpeed);
        rotSpeed /= 3;
        rotSpeed *= 0.5f;
        Vector3 dockPos = target.position;
        Vector3 hoverPos = dockPos + target.up * 5f;
        float rotationAllowance = 0.5f;
        float movementAllowance = 0.05f;

        Debug.Log("Begin Docking Stage 0: Point at HoverPoint");
        
        Vector3 dirToHoverPoint = hoverPos - docker.position;
        Quaternion targetRot = Quaternion.LookRotation(dirToHoverPoint);
        float angle = Quaternion.Angle(transform.rotation, targetRot);
        
        while (angle > rotationAllowance)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotSpeed * Time.deltaTime);
                
            dirToHoverPoint = hoverPos - docker.position;
            targetRot = Quaternion.LookRotation(dirToHoverPoint);
            angle = Quaternion.Angle(transform.rotation, targetRot);
            yield return null;
        }
        
        Debug.Log("Begin Docking Stage 1: Move to HoverPoint");

        float distToHoverPoint = Vector3.Distance(docker.position, hoverPos);
        while (distToHoverPoint > movementAllowance)
        {
            // Scale speed by distance to target, capped at max 50m. Minimum speed 0.05x max, maximum speed 0.5x max
            float speedMultiplier = Remap(Mathf.Clamp(distToHoverPoint, 0, 50), 0, 50, 0.05f, 0.5f);
                
            Vector3 velocity = Vector3.MoveTowards(docker.position, hoverPos, MaxSpeed * speedMultiplier * Time.deltaTime) - docker.position;
            transform.position += velocity;
                
            distToHoverPoint = Vector3.Distance(docker.position, hoverPos);
            yield return null;
        }
        
        Debug.Log("Begin Docking Stage 2: Align with Dock");

        targetRot = target.rotation;
        angle = Quaternion.Angle(transform.rotation, targetRot);
        while (angle > rotationAllowance)
        {
            transform.rotation =
                Quaternion.RotateTowards(transform.rotation, targetRot, rotSpeed * Time.deltaTime);
                
            angle = Quaternion.Angle(transform.rotation, targetRot);
            yield return null;
        }
        
        Debug.Log("Begin Docking Stage 3: Pause for effect");

        yield return new WaitForSeconds(0.5f);
        
        Debug.Log("Begin Docking Stage 4: Lower to Dock");
        
        float distToDock = Vector3.Distance(docker.position, dockPos);
        while (distToDock > movementAllowance)
        {
            // Scale speed by distance to target, capped at max 50m. Minimum speed 0.05x max, maximum speed 0.25x max
            float speedMultiplier = Remap(Mathf.Clamp(distToDock, 0, 0.5f), 0, 50, 0.05f, 0.1f);
                
            Vector3 velocity = Vector3.MoveTowards(docker.position, dockPos, MaxSpeed * speedMultiplier * Time.deltaTime) - docker.position;
            transform.position += velocity;
                
            distToDock = Vector3.Distance(docker.position, dockPos);
            yield return null;
        }
        
        Debug.Log("Docking Complete");
        dock.DockingComplete(Player.Instance.currentShip);
        DockingComplete();
        yield break;
    }
    
    public void CancelDocking ()
    {
        SetState(State.InFlight);
    }

    void DockingComplete()
    {
        state = State.Docked;
    }
    
    public float Remap(float value, float from1, float to1, float from2, float to2) 
    { 
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2; 
    } 

    public float Remap01(float value, float from, float to) 
    { 
        return (value - from) / (to - from); 
    }
}
