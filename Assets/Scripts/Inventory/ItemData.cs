using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Item")]
public class ItemData : SerializedScriptableObject
{
    public string ID;
    public string DisplayName;
    public bool Stackable;
    public int StackLimit;
}
