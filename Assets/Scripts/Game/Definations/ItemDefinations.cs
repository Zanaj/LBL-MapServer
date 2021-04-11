
using System;
using UnityEngine;

public enum UsableItemType
{
    Consumable,
    Equipment,
    Callback,
}

public enum EquipmentSlot
{
    Helmet,
    Shoulder,
    Chest,
    Hands,
    Belt,
    Pants,
    Boots,
    Costume,
    Necklace,
    LeftRing,
    RightRight,
    FirstMainHand,
    FirstOffhand,
    SecondaryMainHand,
    SecondaryOffhand,
}

public enum TimeUnit
{
    Milliseconds,
    Seconds,
    Minutes,
    Hours,
}

[Serializable]
public struct StatPair
{
    [SerializeField]
    public Stat stat;

    [SerializeField]
    public float value;

    [SerializeField]
    public bool isPerminent;

    public StatPair(Stat stat, float value, bool isPerminent = false)
    {
        this.stat = stat;
        this.value = value;
        this.isPerminent = isPerminent;
    }
}

public enum CooldownCategory
{
    Unknown,
    Potoin,
    Foods,
    Equipment,
    Skills,
}