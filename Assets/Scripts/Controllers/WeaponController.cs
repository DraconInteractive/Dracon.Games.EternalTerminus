using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponController : MonoBehaviour
{
    public WeaponComponent[] primaryWeapons;
    public WeaponComponent[] secondaryWeapons;

    private void Start()
    {
        SetupInput();
    }

    private void Update()
    {
        UpdateLog();
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

    #region  Input
    void SetupInput()
    {
        var input = InputController.Instance;
        input.FromID("Primary Fire").inputEvent += PrimaryFireHandler;
        input.FromID("Secondary Fire").inputEvent += SecondaryFireHandler;
    }

    void PrimaryFireHandler(InputController.Context ctx, InputController.InputEventType evt)
    {
        if (evt == InputController.InputEventType.Down)
        {
            foreach (var weaponComponent in primaryWeapons)
            {
                weaponComponent.OnInputDown();
            }
        }
        else if (evt == InputController.InputEventType.Up)
        {
            foreach (var weaponComponent in primaryWeapons)
            {
                weaponComponent.OnInputRelease();
            }
        }
    }

    void SecondaryFireHandler(InputController.Context ctx, InputController.InputEventType evt)
    {
        if (evt == InputController.InputEventType.Down)
        {
            foreach (var weaponComponent in secondaryWeapons)
            {
                weaponComponent.OnInputDown();
            }
        }
        else if (evt == InputController.InputEventType.Up)
        {
            foreach (var weaponComponent in secondaryWeapons)
            {
                weaponComponent.OnInputRelease();
            }
        }
    }
    
    #endregion

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
    
    public void SetShip(Ship newShip)
    {
        newShip.onAttachComponent += CheckNewComponent;
        CheckNewComponent(null, null);
    }

    void CheckNewComponent(ShipComponentAnchor anchor, ShipComponent component)
    {
        if (component is WeaponComponent || component == null)
        {
            var weapons = Player.Instance.currentShip.GetComponentsOfType<WeaponComponent>();
            primaryWeapons = weapons.Where(x => x.tags.Contains(ShipComponent.Tag.Primary)).ToArray();
            secondaryWeapons = weapons.Where(x => x.tags.Contains(ShipComponent.Tag.Secondary)).ToArray();
        }
    }

    void UpdateLog()
    {
        string log = "<b>Weapon Controller</b>\n";
        log += $"# Primary Weapons: {primaryWeapons.Length}\n";
        log += $"# Secondary Weapons: {secondaryWeapons.Length}\n";
        DLog.Instance.AddOrUpdate(this, log);
    }
}
