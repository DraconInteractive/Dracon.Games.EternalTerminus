using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Ship")]
public class ShipData : ScriptableObject
{
    public string displayName;
    [Header("Flight")] 
    public float acceleration;
    public float maxSpeed;
    public float pitchSpeed;
    public float rollSpeed;
    public float yawSpeed;
    [Header("Feel")] 
    public float throttleDeadzone = 0.1f;

    public Vector3 cameraOffset; 
}
