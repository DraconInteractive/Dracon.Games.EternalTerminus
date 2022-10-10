using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public class Ship : MonoBehaviour
{
    public BaseShipController controller;
    public bool controlledByPlayer;
    
    public ShipData data;
    public ShipComponentAnchor[] Anchors;
    public Dock dockedAt;
    
    private Transform cachedTransform;
    private Rigidbody rb;
    
    #region Flight Variables
    public enum FlightState
    {
        Fixed,
        Floating,
        Docking,
        Docked,
        InFlight
    }
    
    [FoldoutGroup("Flight")]
    public FlightState flightState;
    private float currentSpeed;
    private Coroutine dockingRoutine;
    private ShipComponent dockingComponent;

    [FoldoutGroup("Flight/Main"), SerializeField, ReadOnly]
    private float baseAcceleration, accelerationModifier;
    [ShowInInspector, FoldoutGroup("Flight/Properties")]
    public float Acceleration
    {
        get
        {
            return baseAcceleration + accelerationModifier;
        }
    }
    
    [ShowInInspector, FoldoutGroup("Flight/Properties")]
    public float Speed
    {
        get
        {
            return currentSpeed;
        }
    }
    
    [FoldoutGroup("Flight/Main"), SerializeField, ReadOnly]
    private float baseMaxSpeed, maxSpeedModifier;
    [ShowInInspector, FoldoutGroup("Flight/Properties")]
    public float MaxSpeed
    {
        get
        {
            return baseMaxSpeed + maxSpeedModifier;
        }
    }
    [ShowInInspector, FoldoutGroup("Flight/Properties")]
    public float Throttle
    {
        get
        {
            
            if (Mathf.Abs(ThrottleIn) < throttleDeadzone)
            {
                return 0;
            }
            return ThrottleIn;
        }
    }
    [FoldoutGroup("Flight/Main"), SerializeField, ReadOnly]
    private float throttleDeadzone;
    [ShowInInspector, FoldoutGroup("Flight/Properties")]
    public float TargetSpeed
    {
        get
        {
            //float tSpeed = Mathf.Lerp(0, MaxSpeed, Throttle);
            return MaxSpeed * Throttle;
        }
    }
    [FoldoutGroup("Flight/Rotation Speed"), SerializeField, ReadOnly]
    private float yawSpeed, rollSpeed, pitchSpeed;
    
    [FoldoutGroup("Flight/Input"), ReadOnly]
    public float ThrottleIn, PitchIn, YawIn, RollIn;
    #endregion

    #region Weapon Variables

    [FoldoutGroup("Weapons"), ReadOnly]
    public WeaponComponent[] primaryWeapons;
    [FoldoutGroup("Weapons"), ReadOnly]
    public WeaponComponent[] secondaryWeapons;

    #endregion

    #region Targeting Variables
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
    [FoldoutGroup("Targeting")]
    public float scanRange;
    [FoldoutGroup("Targeting"), ReadOnly]
    public bool trackingTargets;
    public (BaseTargetable, float) trackedTarget;
    [FoldoutGroup("Targeting"), ReadOnly]
    public TargetState targetingState;

    #endregion
    public delegate void OnAttachComponent(ShipComponentAnchor anchor, ShipComponent component);
    public OnAttachComponent onComponentAttached;

    #region Core

    private void Awake()
    {
        cachedTransform = transform;
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        SetupAnchors();
        SetupFlight();
    }

    private void Update()
    {
        TargetingUpdate();
    }

    private void FixedUpdate()
    {
        switch (flightState)
        {
            case FlightState.InFlight:
                FlightTick();
                break;
            case FlightState.Floating:
                currentSpeed = Mathf.MoveTowards(currentSpeed, 0, baseAcceleration * Time.deltaTime);
                break;
            default:
                currentSpeed = 0;
                break;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (UnityEditor.EditorApplication.isPlaying)
        {
            Gizmos.DrawWireSphere(transform.position, MaxWeaponRange());
        }
    }
#endif
    #endregion
    
    public void Possess()
    {
        SetController<ShipController_Player>();
        controlledByPlayer = true;
    }

    public void Release()
    {
        SetController<ShipController_AI>();
        controlledByPlayer = false;
    }

    public void SetController<T>() where T : BaseShipController
    {
        if (controller != null)
        {
            controller.Deinitialize();
        }

        controller = gameObject.AddComponent<T>();
        controller.Initialize(this);
    }
    
    void SetupFlight()
    {
        baseAcceleration = data.acceleration;
        baseMaxSpeed = data.maxSpeed;
        yawSpeed = data.yawSpeed;
        pitchSpeed = data.pitchSpeed;
        rollSpeed = data.rollSpeed;
        throttleDeadzone = data.throttleDeadzone;
    }
    
    void SetupAnchors()
    {
        // Find all pre-set components and call the event so hooks can find them
        foreach (var anchor in Anchors)
        {
            ShipComponentAnchor tempAnchor = anchor;
            anchor.onComponentAttached += ComponentAttachedHandler;
            anchor.ship = this;
            if (anchor.component != null)
            {
                //tempAnchor.onComponentAttached?.Invoke(tempAnchor.component);
                tempAnchor.AttachComponent(tempAnchor.component);
            }
        }
        UpdateLog();
    }

    #region Components

    void ComponentAttachedHandler(ShipComponentAnchor anchor, ShipComponent component)
    {
        if (component is EngineComponent)
        {
            AssessEngines();
        }
        else if (component.tags.Contains(ShipComponent.Tag.Docking))
        {
            dockingComponent = component;
        }
        else if (component is WeaponComponent)
        {
            AssessWeapons();
        }
        
        onComponentAttached?.Invoke(anchor, component);
    }

    [Button]
    public void AssessEngines()
    {
        var engines = GetComponentsOfType<EngineComponent>();
            
        accelerationModifier = 0;
        maxSpeedModifier = 0;
            
        if (engines == null || engines.Count == 0)
        {
            return;
        }

        foreach (var engine in engines)
        {
            accelerationModifier += engine.acceleration;
            maxSpeedModifier += engine.maxSpeed;
        }
    }

    [Button]
    public void AssessWeapons()
    {
        var weapons = Player.Instance.currentShip.GetComponentsOfType<WeaponComponent>();
        primaryWeapons = weapons.Where(x => x.tags.Contains(ShipComponent.Tag.Primary)).ToArray();
        secondaryWeapons = weapons.Where(x => x.tags.Contains(ShipComponent.Tag.Secondary)).ToArray();
    }
    
    public List<T> GetComponentsOfType<T>() where T : ShipComponent
    {
        List<T> temp = new List<T>();
        foreach (var shipComponentAnchor in Anchors)
        {
            if (shipComponentAnchor.component is T match)
            {
                temp.Add(match);
            }
        }

        return temp;
    }

    #endregion
    
    #region Flight

    void FlightTick()
    {
        currentSpeed = Mathf.MoveTowards(currentSpeed, TargetSpeed, Acceleration * Time.fixedDeltaTime);
        
        Vector3 targetPos = cachedTransform.position + cachedTransform.forward * (currentSpeed * Time.deltaTime);
        rb.MovePosition(targetPos);
        
        Vector3 rotationSolver = new Vector3();

        float speedRotationMultiplier = Remap(Mathf.Abs(currentSpeed), 0, MaxSpeed, 0f, 0.8f);
        Vector3 rotDelta = new Vector3()
        {
            x = PitchIn * pitchSpeed,
            y = YawIn * yawSpeed,
            z = RollIn * rollSpeed
        };
        rotDelta *= (1 - speedRotationMultiplier) * Time.deltaTime;
        rotationSolver.x += rotDelta.x;
        rotationSolver.y += rotDelta.y;
        rotationSolver.z += rotDelta.z;
        rb.MoveRotation(cachedTransform.localRotation * Quaternion.Euler(rotationSolver));
    }
    
    public void SetFlightState(FlightState newState)
    {
        if (newState != FlightState.Docking && dockingRoutine != null)
        {
            StopCoroutine(dockingRoutine);
        }

        flightState = newState;
        if (controlledByPlayer)
            InputController.Instance.AssessContext();
    }
    
    public void StartDocking (Dock dock)
    {
        SetFlightState(FlightState.Docking);
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
        
        targetRot = target.rotation;
        angle = Quaternion.Angle(transform.rotation, targetRot);
        while (angle > rotationAllowance)
        {
            transform.rotation =
                Quaternion.RotateTowards(transform.rotation, targetRot, rotSpeed * Time.deltaTime);
                
            angle = Quaternion.Angle(transform.rotation, targetRot);
            yield return null;
        }
        
        yield return new WaitForSeconds(0.5f);
        
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
        
        dock.DockingComplete(Player.Instance.currentShip);
        SetFlightState(FlightState.Docked);
        yield break;
    }
    
    public void StartUndocking()
    {
        StartCoroutine(UndockingRoutine());
    }
    
    IEnumerator UndockingRoutine()
    {
        Dock currentDock = Player.Instance.currentShip.dockedAt;
        if (currentDock == null)
        {
            yield break;
        }
        SetFlightState(FlightState.Docking);
        
        Transform target = currentDock.dockingPoint;
        if (dockingComponent == null)
        {
            Debug.LogWarning("Undocking Cancelled: Component Missing");
            CancelDocking();
        }

        Transform docker = dockingComponent.transform;

        // setup useful variables
        Vector3 dockPos = target.position;
        Vector3 hoverPos = dockPos + target.up * 5f;
        float movementAllowance = 0.05f;
        
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
        
        SetFlightState(FlightState.InFlight);
        yield break;
    }
    
    public void CancelDocking ()
    {
        SetFlightState(FlightState.InFlight);
    }

    #endregion

    #region Weapons

    public float MaxWeaponRange()
    {
        float max = 0;
        foreach (var weapon in primaryWeapons)
        {
            if (weapon.data.range > max)
            {
                max = weapon.data.range;
            }
        }
        foreach (var weapon in secondaryWeapons)
        {
            if (weapon.data.range > max)
            {
                max = weapon.data.range;
            }
        }

        return max;
    }

    public float MinWeaponDot()
    {
        float min = 0;
        foreach (var weapon in primaryWeapons)
        {
            if (weapon.data.trackingFOV < min)
            {
                min = weapon.data.trackingFOV;
            }
        }
        foreach (var weapon in secondaryWeapons)
        {
            if (weapon.data.trackingFOV < min)
            {
                min = weapon.data.trackingFOV;
            }
        }

        return min;
    }

    public void FirePrimary()
    {
        foreach (var weaponComponent in primaryWeapons)
        {
            weaponComponent.TryFire();
        }
    }
    
    public void StartFire_Primary()
    {
        foreach (var weaponComponent in primaryWeapons)
        {
            weaponComponent.OnInputRelease();
        }
    }

    public void EndFire_Primary()
    {
        foreach (var weaponComponent in primaryWeapons)
        {
            weaponComponent.OnInputRelease();
        }
    }

    public void FireSecondary()
    {
        foreach (var weaponComponent in secondaryWeapons)
        {
            weaponComponent.TryFire();
        }
    }
    
    public void StartFire_Secondary()
    {
        foreach (var weaponComponent in secondaryWeapons)
        {
            weaponComponent.OnInputRelease();
        }
    }

    public void EndFire_Secondary()
    {
        foreach (var weaponComponent in secondaryWeapons)
        {
            weaponComponent.OnInputRelease();
        }
    }
    
    #endregion

    #region Targeting

    private void TargetingUpdate()
    {
        if (!trackingTargets)
        {
            targetingState = TargetState.TrackingInactive;
            return;
        }
        
        if (trackedTarget.Item1 != null)
        {
            Vector3 targetDir = trackedTarget.Item1.Position() - transform.position;
            float dot = Vector3.Dot(transform.forward, targetDir.normalized);
            float currentTrackingLevel = trackedTarget.Item2;
            if (dot > MinWeaponDot() && Vector3.Distance(transform.position, trackedTarget.Item1.Position()) < MaxWeaponRange())
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
                targetingState = TargetState.TargetLocked;
            }
            else
            {
                targetingState = TargetState.TargetAcquired;
            }
        }
        else
        {
            targetingState = TargetState.NoTarget;
        }
    }
    
    public void ToggleTargetTracking()
    {
        trackingTargets = !trackingTargets;
        trackedTarget = (null, 0);
    }

    public enum FindTargetMethod
    {
        Forward,
        Closest
    }
    
    public void FindTarget(FindTargetMethod method)
    {
        Debug.Log("Finding target, method: " + method);
        if (!trackingTargets)
        {
            Debug.Log("Not tracking");
            trackedTarget = (null, 0);
            return;
        }

        BaseTargetable tempTarget = null;
        var viableTargets = BaseTargetable.All
            .Where(x => Vector3.Distance(x.Position(), transform.position) < scanRange);
        
        if (method == FindTargetMethod.Closest)
        {
            float maxRange = scanRange;
            Vector3 pos = transform.position;
            foreach (var target in viableTargets)
            {
                float dist = Vector3.Distance(target.Position(), pos);
                if (dist < maxRange)
                {
                    maxRange = dist;
                    tempTarget = target;
                }
            }
        }
        else if (method == FindTargetMethod.Forward)
        {
            float minDot = 0;
            
            foreach (var target in viableTargets)
            {
                Vector3 dir = target.Position() - transform.position;
                float dot = Vector3.Dot(transform.forward, dir.normalized);
                if (dot > minDot)
                {
                    minDot = dot;
                    tempTarget = target;
                }
            }
        }
        trackedTarget = (tempTarget, 0);
    }

    #endregion
    void UpdateLog()
    {
        string log = $"<b>Ship - {data.displayName}</b>\n";
        log += $"# Anchors: {Anchors.Length}\n";
        log += $"Add flight stats here!";
        DLog.Instance.AddOrUpdate(this, log);
    }

    #region Tools

#if UNITY_EDITOR
    [ContextMenu("Find Anchors")]
    public void EDITOR_FindChildAnchors()
    {
        Anchors = GetComponentsInChildren<ShipComponentAnchor>();
    }
#endif
    
    public float Remap(float value, float from1, float to1, float from2, float to2) 
    { 
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2; 
    } 

    public float Remap01(float value, float from, float to) 
    { 
        return (value - from) / (to - from); 
    }

    #endregion

}