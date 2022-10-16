using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Asteroid : Targetable
{
    public Inventory inventory;
    
    public override string ID()
    {
        return "Asteroid";
    }

    protected override void Death(object instigator)
    {
        base.Death(instigator);
        ItemContainer container = Instantiate(Resources.Load("ItemContainer"), transform.position, transform.rotation).GetComponent<ItemContainer>();
        container.items = inventory.items;
        
        Destroy(this.gameObject);
    }
}
