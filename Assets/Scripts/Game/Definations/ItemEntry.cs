using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class ItemEntry
{
    public int itemID;
    public ItemData data;
    public int amount;
    public int leftFromStack { get { return data.maxStack - amount; } }

    public ItemEntry(ItemData data, int amount)
    {
        this.data = data;
        this.amount = amount;
        if (ContentDatabase.instance == null)
            itemID = -1;

        if (!ContentDatabase.instance.items.Contains(data))
            itemID = -1;

        if (itemID != -1)
            itemID = ContentDatabase.instance.items.ToList().FindIndex(x => x == data);
    }

    public static bool isStackableWith(ItemEntry first, ItemEntry stacking)
    {
        if (first.data != stacking.data)
            return false;

        if (!first.data.isStackable)
            return false;

        if (first.amount + stacking.amount > first.data.maxStack)
            return false;

        return true;
    }

}
