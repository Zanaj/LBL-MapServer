using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Content/ConsumableItem", order = 1)]
public class ConsumableItem : UsableItem
{
    [Tooltip("This sets which cooldown this shares with. You have to consider that things that aren't grouped together can potentally be eaten in same game tick!")]
    public CooldownCategory cooldownType;

    [Tooltip("Cooldown in miliseconds. Put this to 0 for no cooldown or. Remember to set proper cooldown category!")]
    public int millisecondsCooldown;

    [Tooltip("How long should this stay effect stay on for in miliseconds?")]
    public int millisecondsBuffTimer;

    [Tooltip("Use this for things like a pill that gives +1 str forever or if you wish to heal 30 hp.")]
    public bool isPerment;

    [SerializeField]
    public StatPair[] buffs;

    public override UsableItemType type => UsableItemType.Consumable;

    public override bool CanUseItem(Entity activator)
    {
        if (!activator.cooldowns.ContainsKey(cooldownType))
            return true;

        DateTime timeLastUsage = activator.cooldowns[cooldownType];
        TimeSpan span = DateTime.Now - timeLastUsage;
        return span.TotalSeconds >= millisecondsCooldown;
    }

    public override void UseItem(Entity activator)
    {
        if (!CanUseItem(activator))
            return;

        //TODO: check if item is in inventory

        if(activator is Player)
        {
            //TODO: return to the player that they did the thing
        }

        for (int i = 0; i < buffs.Length; i++)
        {
            StatPair buff = buffs[i];
            if (buff.isPerminent)
            {
                activator.ModifyStat(buff.stat, buff.value);
                return;
            }

            Condition condition = new Condition()
            {
                originType = 0,
                originID = (int)buff.stat,
                amount = buff.value,
                recievedAt = DateTime.Now,
                type = ConditionType.Stat,
                conditionedStat = buff.stat,
            };

            activator.ApplyCondition(condition);
        }
    }
}
