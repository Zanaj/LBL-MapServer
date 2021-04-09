using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class Player
{
    public int inventorySize;
    public List<ItemEntry> inventory = new List<ItemEntry>();

    public bool CanAddItems(params ItemEntry[] items)
    {
        //If we pretend all items are nonstackable then well and theres enough space go ahead.
        if (inventory.Count + items.Length <= inventorySize)
            return true;

        for (int i = items.Length - 1; i >= 0; i--)
        {
            ItemEntry item = items[i];

            //This loop we check for stacks if its not stackable no need to check.
            if (!item.data.isStackable)
                continue;

            //If this item doesn't exist in the inventory no need to check if it can be stacked
            if (!inventory.Exists(x => x.data == item.data))
                continue;

            //Get all entries where this item is
            ItemEntry[] allItems = inventory
                .Where(x => x.data == item.data)
                .ToArray();

            //Sum all space left from the entries.
            int spaceLeftInStacks = allItems.Sum(x => x.leftFromStack);

            //If theres less space than required then return false
            if (spaceLeftInStacks < item.amount)
                return false;
        }

        return true;
    }

    public void AddItems(params ItemEntry[] items)
    {
        if (!CanAddItems(items))
            return;

        foreach (ItemEntry item in items)
        {
            
        }
    }
}
