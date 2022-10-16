using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class Enemy : Targetable
{
    [ReadOnly]
    private Rigidbody rb;

    protected override void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    protected override void Start()
    {
        actions = new []
        {
            new ContextAction()
            {
                ID = "Taunt",
                Action = () => TauntAction()
            },
            new ContextAction()
            {
                ID = "Set Hostile",
                Action = () => Debug.Log("Fake Action")
            },
            new ContextAction()
            {
                ID = "Begin Comms",
                Action = () => Debug.Log("Fake Action")
            }
        };
    }

    private void Update()
    {
        velocity = rb.velocity;
    }

    public void TauntAction()
    {
        Debug.Log("Fuck you");
    }
}