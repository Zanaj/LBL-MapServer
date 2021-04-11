using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UsableItem : ItemData
{
    public abstract UsableItemType type { get; }

    public abstract bool CanUseItem(Entity activator);

    public abstract void UseItem(Entity activator);
}
