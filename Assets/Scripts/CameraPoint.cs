using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CameraPoint : MonoBehaviour
{
    public UnityEvent onSetActive, onSetInactive;

    public BaseHUD HUD;

    [Space] 
    public float baseMovementSpeed = 1;
    public float maxMovementSpeed = 10;
    public float rotationSpeed;
    
    public virtual void TransformCamera(Transform cam)
    {
        Ship ship = Player.Instance.currentShip;
        float currentSpeed = Mathf.Abs(ship.Speed);
        float maxSpeed = ship.MaxSpeed;
        float r = currentSpeed / maxSpeed;
        cam.position = Vector3.Lerp(cam.position, transform.position, Mathf.Lerp(baseMovementSpeed, maxMovementSpeed, r) * Time.deltaTime);
        cam.rotation = Quaternion.Lerp(cam.rotation, transform.rotation, rotationSpeed * Time.deltaTime);
    }
    
    public void SetActive()
    {
        HUD.Activate();
        onSetActive?.Invoke();
    }

    public void SetInactive()
    {
        HUD.Deactivate();
        onSetInactive?.Invoke();
    }
}
