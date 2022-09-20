using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_ContextMenuItem : MonoBehaviour
{
    public Action onPress;
    public TMP_Text text;
    public GameObject highlight;

    public void AddHighlight()
    {
        highlight.gameObject.SetActive(true);
    }

    public void RemoveHighlight()
    {
        highlight.gameObject.SetActive(false);
    }
}
