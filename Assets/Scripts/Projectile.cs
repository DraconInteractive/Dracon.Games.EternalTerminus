using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float movementSpeed = 5;
    public float TTL = 5;

    private bool usePhysics;
    private Rigidbody rb;

    private void Start()
    {
        var _rb = GetComponent<Rigidbody>();
        if (_rb != null)
        {
            rb = _rb;
            usePhysics = true;
        }
    }

    private void Update()
    {
        if (!usePhysics)
        {
            transform.position += transform.forward * movementSpeed * Time.deltaTime;
        }
        
        TTL -= Time.deltaTime;
        if (TTL < 0)
        {
            Destroy(this.gameObject);
        }
    }

    private void FixedUpdate()
    {
        if (usePhysics)
        {
            rb.MovePosition(transform.position + transform.forward * movementSpeed * Time.fixedDeltaTime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(this.gameObject);
    }
}
