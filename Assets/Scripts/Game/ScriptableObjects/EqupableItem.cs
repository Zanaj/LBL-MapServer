using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipableItem : UsableItem
{
    public int levelRequirement;
    public override UsableItemType type => UsableItemType.Equipment;

    [SerializeField]
    public StatPair[] buffs;

    public EquipmentSlot slot;

    public override bool CanUseItem(Entity activator)
    {
        if (!(activator is Player))
            return false;

        Player player = (Player)activator;
        if (player.level < levelRequirement)
            return false;

        ItemEntry item = new ItemEntry(this, 1);
        if (player.HaveItem(item) != InventoryErrorCode.Success)
            return false;

        return true;
    }

    public override void UseItem(Entity activator)
    {
        if (CanUseItem(activator))
        {
            Player player = (Player)activator;

        }
    }
}
