using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipableItem : UsableItem
{
    public int levelRequirement;
    public override UsableItemType type => UsableItemType.Equipment;

    [SerializeField]
    public StatPair[] buffs;

    public override bool CanUseItem(Entity activator)
    {
        //TODO: is there any reason for equipment not being able to unequip???
        if (!(activator is Player))
            return false;

        Player player = (Player)activator;
        if (player.level < levelRequirement)
            return false;

        return true;
    }

    public override void UseItem(Entity activator)
    {
        if (CanUseItem(activator))
        {

        }
    }
}
