using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Pawn : SerializedMonoBehaviour
{
    public bool alive;
    
    [ReadOnly]
    public float currentHealth;
    public float maxHealth;

    public EventHandler onDeath;
    protected virtual void Awake()
    {
    }

    protected virtual void Start()
    {
        alive = true;
        currentHealth = maxHealth;
    }

    protected virtual void OnEnable()
    {
    }
    
    protected virtual void OnDisable()
    {
    }
    
    public virtual string ID()
    {
        return this.gameObject.name;
    }
    
    public virtual void OnHit(object instigator, float damage)
    {
        if (!alive)
        {
            return;
        }
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Death(instigator);
        }
    }

    protected virtual void Death(object instigator)
    {
        alive = false;
    }
}
