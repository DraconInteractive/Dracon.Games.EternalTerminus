using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CameraPoint : MonoBehaviour
{
    public UnityEvent onSetActive, onSetInactive;

    public BaseHUD HUD;

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
