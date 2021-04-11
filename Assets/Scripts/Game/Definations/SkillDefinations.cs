using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ConditionType
{
    DamageVitalOverTime,
    ApplyDebuffOverTime,
    Stat,
}

public struct Condition
{
    public ConditionType type;
    public Stat conditionedStat;
    public float amount;
    public int originType;
    public int originID;

    public DateTime recievedAt;
}

public interface IInteractable
{
    bool CanInteract(Entity activator, List<string> extraInfo);
    void InteractWith(Entity activator, List<string> extraInfo);
}