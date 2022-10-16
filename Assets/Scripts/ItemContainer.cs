using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Serialization;
using UnityEngine;

public class ItemContainer : Targetable
{
    public Dictionary<Item, int> items = new Dictionary<Item, int>();

    public override string ID()
    {
        return "Item Container";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody.TryGetComponent(out Ship ship))
        {
            TransferItems(ship);   
        }
    }

    void TransferItems(Ship ship)
    {
        foreach (var key in items.Keys.ToList())
        {
            var amount = items[key];
            if (amount == 0)
            {
                continue;
            }
                
            int leftOver = ship.inventory.Add(key, items[key]);
            items[key] = leftOver;
        }

        if (items.All(x => x.Value == 0))
        {
            Destroy(this.gameObject);
        }
    }
}
