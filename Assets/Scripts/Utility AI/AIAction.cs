using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public abstract class AIAction : SerializedScriptableObject
{
    public abstract bool CanExecute(Ship ship);
    public abstract float GetScore(Ship ship);
    public abstract void Execute(Ship ship);
    
    #if UNITY_EDITOR
    [MenuItem("Dracon/Create AI Actions")]
    public static void CreateActionArchetypes()
    {
        string actionsPath = "Assets/Data/AIActions";

        List<System.Type> actionClasses = typeof(AIAction)
            .Assembly.GetTypes()
            .Where(type => type.IsSubclassOf(typeof(AIAction)) && !type.IsAbstract)
            .ToList();

        foreach (System.Type actionClass in actionClasses)
        {
            string assetPath = actionsPath + actionClass.Name + ".asset";
            Object existingAsset = AssetDatabase.LoadAssetAtPath(assetPath, actionClass);
            if (existingAsset == null)
            {
                existingAsset = CreateInstance(actionClass);
                AssetDatabase.CreateAsset(existingAsset, assetPath);
                Debug.Log("Asset Created: " + assetPath);
            }
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    #endif
};
