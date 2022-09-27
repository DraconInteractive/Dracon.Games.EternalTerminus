using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FlowHost : SerializedMonoBehaviour
{
    public static List<FlowHost> All = new List<FlowHost>();
    public static List<FlowHost> Running = All.Where(x => x.state == State.Running).ToList();
    public enum Trigger
    {
        Manual,
        OnAwake,
        OnStart
    }

    public enum State
    {
        Idle,
        Running, 
        Complete,
        Cancelled,
        Starting
    }

    [ReadOnly]
    public State state;
    public Trigger startTrigger;

    [Space]
    public FlowHost setupFlow;
    public bool callSetupOnRun = true;
    
    [Space]
    public bool loop;

    [Space,OdinSerialize, System.NonSerialized]
    public FlowAction[] flowActions;
    
    Coroutine actionRoutine;
    
    private void Awake()
    {
        SetupActions();

        if (startTrigger == Trigger.OnAwake)
        {
            Run();
        }
    }

    private void Start()
    {
        if (startTrigger == Trigger.OnStart)
        {
            Run();
        }
    }

    private void OnEnable()
    {
        All.Add(this);
    }

    private void OnDisable()
    {
        All.Remove(this);
    }

    void SetupActions ()
    {
        foreach (var action in flowActions)
        {
            action.Setup(this);
        }
    }

    [ResponsiveButtonGroup()]
    public void Setup()
    {
        if (actionRoutine != null)
        {
            StopCoroutine(actionRoutine);
        }

        actionRoutine = StartCoroutine(SetupRoutine(false));
    }
    
    [ResponsiveButtonGroup()]
    public void Run ()
    {
        if (actionRoutine != null)
        {
            StopCoroutine(actionRoutine);
        }
        actionRoutine = StartCoroutine(RunRoutine());
    }

    public void Stop ()
    {
        state = State.Cancelled;
    }

    IEnumerator SetupRoutine(bool affectState)
    {
        if (affectState)
        {
            state = State.Starting;
        }
        if (setupFlow != null)
        {
            setupFlow.Run();
            yield return null;
            while (setupFlow.state == State.Running)
            {
                yield return null;
            }
        }

        yield break;
    }
    IEnumerator RunRoutine ()
    {
        if (callSetupOnRun)
        {
            yield return StartCoroutine(SetupRoutine(true));
        }
        
        state = State.Running;
        foreach (var action in flowActions)
        {
            if(action.skip)
            {
                action.state = FlowAction.State.Complete;
                continue;
            }
            action.Begin();
            while (action.state != FlowAction.State.Complete)
            {
                if (state == State.Cancelled)
                {
                    action.Cancel();
                    yield break;
                }
                yield return null;
            }
        }
        if (state == State.Cancelled)
        {
            yield break;
        }
        Complete();
    }

    void Complete ()
    {
        state = State.Complete;
        actionRoutine = null;
        if (loop)
        {
            Run();
        }
    }

    public void ResetState ()
    {
        state = State.Idle;

        foreach (var action in flowActions.Reverse())
        {
            switch (action.state)
            {
                case FlowAction.State.Running:
                    action.Cancel();
                    action.Reset();
                    break;
                case FlowAction.State.Complete:
                    action.Reset();
                    break;
                case FlowAction.State.Cancelled:
                    action.Reset();
                    break;
            }

        }
    }
    
    
}
