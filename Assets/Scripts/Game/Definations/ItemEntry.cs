using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEntry
{
    public ItemData data;
    public int amount;
    public int leftFromStack { get { return data.maxStack - amount; } }

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
