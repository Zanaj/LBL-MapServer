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

public enum InteractionType
{
    Unknown,
    Attack,
    Examine,
    Talk,
    Shop,
    NpcOption,
    UseItem,
    EatItem,
    EquipItem,
    DropItem,
}

public class Entity : EntityStats
{
    public virtual EntityType type { get; }
    public string displayName;
    public string entityGUID;
    public InteractionType[] options;
    public virtual bool isInteractable { get; }
    public Entity target;

    private void Awake()
    {
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
}
