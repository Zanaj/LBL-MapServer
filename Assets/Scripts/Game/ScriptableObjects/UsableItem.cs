using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UsableItemType
{
    Consumable,
    Equipment,
    Callback,
}

public abstract class UsableItem : ItemBase
{
    public abstract UsableItemType type { get; }

    public abstract bool CanUseItem(Entity activator);

    public abstract void UseItem(Entity activator);
}
