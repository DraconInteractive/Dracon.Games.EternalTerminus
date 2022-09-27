using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FAction_TypeUIText : FlowAction
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
    public float typeRate = 0.1f;

    public override void Begin()
    {
        base.Begin();

        instigator.StartCoroutine(Run());
    }

    IEnumerator Run ()
    {
        if (uiTextType == UITextType.UnityUI)
        {
            uiText.text = "";
            foreach (var c in text)
            {
                uiText.text += c;
                yield return new WaitForSeconds(typeRate);
                if (state == State.Cancelled)
                {
                    yield break;
                }
            }
        }
        else if (uiTextType == UITextType.TMPro)
        {
            tmpText.text = "";
            foreach (var c in text)
            {
                tmpText.text += c;
                yield return new WaitForSeconds(typeRate);
                if (state == State.Cancelled)
                {
                    yield break;
                }
            }
        }
        if (state == State.Cancelled)
        {
            yield break;
        }
        Complete();
        yield break;
    }
}
