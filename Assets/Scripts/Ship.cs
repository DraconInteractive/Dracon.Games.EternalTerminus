using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class Ship : MonoBehaviour
{
    public ShipData data;
    
    public ShipComponentAnchor[] Anchors;

    public delegate void OnAttachComponent(ShipComponentAnchor anchor, ShipComponent component);
    public OnAttachComponent onAttachComponent;

    private void Start()
    {
        SetupAnchors();
        onAttachComponent += (anchor, component) => UpdateLog();
    }

    void SetupAnchors()
    {
        // Find all pre-set components and call the event so hooks can find them
        foreach (var anchor in Anchors)
        {
            ShipComponentAnchor tempAnchor = anchor;
            anchor.onComponentAttached += c => onAttachComponent(tempAnchor, c);
            if (anchor.component != null)
            {
                onAttachComponent?.Invoke(anchor, anchor.component);
                onAttachComponent?.Invoke(tempAnchor, tempAnchor.component);
            }
        }
        UpdateLog();
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

    void UpdateLog()
    {
        string log = $"<b>Ship - {data.displayName}</b>\n";
        log += $"# Anchors: {Anchors.Length}";
        DLog.Instance.AddOrUpdate(this, log);
    }
    
#if UNITY_EDITOR
    [ContextMenu("Find Anchors")]
    public void EDITOR_FindChildAnchors()
    {
        Anchors = GetComponentsInChildren<ShipComponentAnchor>();
    }
#endif
}
