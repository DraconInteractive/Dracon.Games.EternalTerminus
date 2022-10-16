using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    public float ttl;

    private void Start()
    {
        Invoke(nameof(Die), ttl);
    }

    void Die()
    {
        Destroy(this.gameObject);
    }
}
