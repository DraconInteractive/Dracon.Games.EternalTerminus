using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class Inventory : SerializedMonoBehaviour
{
    public Dictionary<Item, int> items = new Dictionary<Item, int>();

    public bool limitedCapacity;
    [ShowIf("limitedCapacity")] 
    public int capacity;

    /// <summary>
    /// See .Add(Item item, int amount) for more details
    /// </summary>
    /// <param name="data"></param>
    /// <param name="amount"></param>
    /// <returns></returns>
    public int AddFromData(ItemData data, int amount = 1)
    {
        Item i = new Item()
        {
            data = data
        };
        return Add(i, amount);
    }
    
    /// <summary>
    /// Add x amount of item to inventory
    /// </summary>
    /// <param name="item">Item to add</param>
    /// <param name="amount">Amount to add</param>
    /// <returns>Returns the amount of items unable to be added to inventory.
    /// This may be due to capacity or stack limitations</returns>
    public int Add(Item item, int amount = 1)
    {
        int remaining = -1;
        if (item.data.Stackable)
        {
            var stack = items.Keys.FirstOrDefault(x => x.data.ID == item.data.ID);
            if (stack == null)
            {
                if (limitedCapacity && items.Count == capacity)
                {
                    return amount;
                }
                items.Add(item, 0);
                stack = item;
            }
            var stackData = stack.data;
            var currentStackSize = items[stack];
            // TODO instead of just returning due to stack limit, check to see if we can add another stack. 
            // Will require changes above to retrieving stack as well. Will have to get *all* instances with ID, and find one with space
            // Possibly should also separate out stack and capacity checks to other methods
            if ((currentStackSize + amount) > stackData.StackLimit)
            {
                remaining = Mathf.Abs(stack.data.StackLimit - (currentStackSize + amount));
                items[stack] = stack.data.StackLimit;
            }
            else
            {
                remaining = 0;
                items[stack] = currentStackSize + amount;
            }
        }
        else
        {
            remaining = 0;
            for (int i = 0; i < amount; i++)
            {
                if (!(limitedCapacity && items.Count == capacity))
                {
                    items.Add(item, 1);
                }
                else
                {
                    remaining++;
                }
            }
        }
        return remaining;
    }

    public int RemoveWithID(string id, int amount)
    {
        int remaining = -1;
        Item item = items.Keys.FirstOrDefault(x => x.data.ID == id);
        if (item != null)
        {
            return Remove(item, amount);
        }
        return remaining;
    }
    
    /// <summary>
    /// Remove x amount of items from inventory
    /// </summary>
    /// <param name="item">Item to remove</param>
    /// <param name="amount">Amount to remove</param>
    /// <returns>
    /// Amount that was not able to be removed.
    /// If you attempt to remove 5, and there are only 3, function will return 2.
    /// Function will return -1 if item does not exist to be removed.
    /// </returns>
    public int Remove(Item item, int amount)
    {
        int remaining = -1;
        if (items.ContainsKey(item))
        {
            if (items[item] >= amount)
            {
                items[item] -= amount;
                remaining = 0;

                if (items[item] == 0)
                {
                    items.Remove(item);
                }
            }
            else
            {
                remaining = Mathf.Abs(items[item] - amount);
                items.Remove(item);
            }
        }
        return remaining;
    }
}
