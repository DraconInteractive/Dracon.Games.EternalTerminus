using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/AI/Archetype")]
public class AIArchetype : SerializedScriptableObject
{
    public AIAction[] locomotionActions;
    public AIAction[] interactionActions;
    
    public float decisionPeriod = 1f;
    public float entropyMin = 0.9f, entropyMax = 1.1f;
}
