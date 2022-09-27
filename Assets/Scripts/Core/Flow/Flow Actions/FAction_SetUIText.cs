using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FAction_SetUIText : FlowAction
{
    public enum UITextType
    {
        UnityUI,
        TMPro
    }
    [EnumToggleButtons]
    public UITextType uiTextType;

    [ShowIf("uiTextType", UITextType.UnityUI)]
    public Text uiText;
    [ShowIf("uiTextType", UITextType.TMPro)]
    public TMPro.TMP_Text tmpText;

    public string text;

    public override void Begin()
    {
        base.Begin();

        switch (uiTextType)
        {
            case UITextType.UnityUI:
                uiText.text = text;
                break;
            case UITextType.TMPro:
                tmpText.text = text;
                break;
        }

        Complete();
    }
}
