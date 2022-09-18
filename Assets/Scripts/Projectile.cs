using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float movementSpeed = 5;
    public float TTL = 5;
    private void Update()
    {
        transform.position += transform.forward * movementSpeed * Time.deltaTime;
        TTL -= Time.deltaTime;
        if (TTL < 0)
        {
            Destroy(this.gameObject);
        }
    }
}
