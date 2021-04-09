using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum EntityType
{
    Unknown,
    Interactable,
    Enemy,
    Player,
    NPC,
    Special,
}

public interface IInteractable
{
    bool CanInteract(Entity activator, List<string> extraInfo);
    void InteractWith(Entity activator, List<string> extraInfo);
}

public enum InteractionType
{
    Unknown,
    Attack,
    Examine,
    Talk,
    Shop,
    NpcOption,
    OpenDoor,
    PickItem,
    UseItem,
    EatItem,
    EquipItem,
    DropItem,
}

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

public class Entity : EntityStats
{
    public virtual EntityType type { get; }
    public string displayName;
    public string entityGUID;
    public InteractionType[] options;
    public virtual bool isInteractable { get; }
    public Entity target;

    public float respawnSeconds;
    public DateTime respawnTimer;
    public Vector3 startPosition;
    public bool needRespawning;

    public Dictionary<CooldownCategory, DateTime> cooldowns;
    
    private void Start()
    {
        cooldowns = new Dictionary<CooldownCategory, DateTime>();

        startPosition = transform.position;

        entityGUID = Guid.NewGuid().ToString();
        EntityManager.instance.entities.Add(this);

        switch (type)
        {
            case EntityType.Unknown:
                break;
            case EntityType.Interactable:
                break;
            case EntityType.NPC:
                NPCManager.instance.NPCS.Add(this);
                break;
            case EntityType.Special:
                break;
            default:
                break;
        }
    }

    private void Update()
    {
        
    }

    public virtual void MakeEntityPacket(BinaryWriter writer)
    {
        writer.Write((int)type);
        writer.Write(displayName);
        writer.Write(entityGUID);
        writer.Write(isInteractable);

        writer.Write(transform.position.x);
        writer.Write(transform.position.y);
        writer.Write(transform.position.z);

        writer.Write(options.Length);
        if (options.Length > 0)
        {
            for (int i = 0; i < options.Length; i++)
            {
                writer.Write((int)options[i]);
            }
        }
    }

    public void ModifyStat(Stat stat, float value)
    {
        float currVal = GetStat(stat, false);
        currVal += value;

        SetStat(stat, value);
    }

    public void ApplyCondition(Condition condition)
    {
        conditions.Add(condition);
    }

    public override void Death()
    {
        base.Death();
        respawnTimer = DateTime.Now;
    }

    public virtual void Respawn()
    {
        needRespawning = false;
        transform.position = startPosition;
    }
}
