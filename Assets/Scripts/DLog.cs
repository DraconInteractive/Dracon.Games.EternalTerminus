using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DLog : Manager<DLog>
{
    public Dictionary<Component, string> Logs = new Dictionary<Component, string>();
    public bool debug = true;
    public TMP_Text text;
    
    public void AddOrUpdate(Component owner, string message)
    {
        if (Logs.ContainsKey(owner))
        {
            Logs[owner] = message;
        }
        else
        {
            Logs.Add(owner, message);
        }

        if (!debug)
        {
            text.text = "";
            return;
        }
        string output = "";
        foreach (var log in Logs)
        {
            output += log.Value + "\n\n";
        }

        text.text = output;
    }

    public void Remove(Component owner)
    {
        if (Logs.ContainsKey(owner))
        {
            Logs.Remove(owner);
        }
    }
}
