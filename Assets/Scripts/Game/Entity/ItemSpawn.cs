using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class ItemSpawn : Entity, IInteractable
{
    public override bool isInteractable => true;
    public override EntityType type => EntityType.Pickup;

    public Player owner;

    public ItemEntry[] inventory;

    public bool shouldDelete;

    public bool isPermentSpawn;
    public TimeUnit timeUnit;
    public int respawnCooldown;

    
    private void Start()
    {
        entityGUID = Guid.NewGuid().ToString();
        EntityManager.instance.itemSpawns.Add(this);
        Sync();
    }
    private void Sync()
    {
        EntitySync sync = new EntitySync();
        sync.entity = this;
        sync.entityType = EntityType.Interactable;
        sync.Serialize();

        if (owner != null)
        {
            int connectionID = NetworkManager.instance.connectionToAccountID.Values.ToList().Find(x => x == owner.characterID);

            NetworkManager.Send(connectionID, sync);
        }

        NetworkManager.instance.SendAll(sync);
    }

    public bool CanInteract(Entity activator, List<string> extraInfo)
    {
        if (shouldDelete)
            return false;

        if (!(activator is Player))
            return false;

        Player player = (Player)activator;
        return player.CanAddItems(inventory) == InventoryErrorCode.Success;
    }

    public void InteractWith(Entity activator, List<string> extraInfo)
    {
        if (!(activator is Player))
            return;

        if (!CanInteract(activator, extraInfo))
            return;

        Player player = (Player)activator;
        player.AddItems(inventory);

        Death();
    }

    private void Update()
    {
        if (shouldDelete)
        {
            TimeSpan span = DateTime.Now - respawnTimer;

            double passedTime = 0;
            switch (timeUnit)
            {
                case TimeUnit.Seconds:
                    passedTime = span.TotalSeconds;
                    break;
                case TimeUnit.Minutes:
                    passedTime = span.TotalMinutes;
                    break;
                case TimeUnit.Hours:
                    passedTime = span.TotalHours;
                    break;
                default:
                    passedTime = span.TotalMilliseconds;
                    break;
            }

            if (passedTime >= respawnCooldown)
                Respawn();
        }
    }

    public override void Death()
    {
        shouldDelete = true;
        EntitySync sync = new EntitySync();
        sync.entity = this;
        sync.entityType = EntityType.Pickup;
        sync.Serialize();
        NetworkManager.instance.SendAll(sync);

        if (!isPermentSpawn)
        {
            EntityManager.instance.itemSpawns.Remove(this);
            Destroy(this.gameObject);
        }
    }

    public override void Respawn()
    {
        shouldDelete = false;
        Sync();
    }

    public override void MakeEntityPacket(BinaryWriter writer)
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

        writer.Write(shouldDelete);

        if (shouldDelete)
            return;
        
        writer.Write(inventory.Length);
        if(inventory.Length > 0)
        {
            for (int i = 0; i < inventory.Length; i++)
            {
                writer.Write(inventory[i].itemID);
                writer.Write(inventory[i].amount);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, NetworkManager.instance.MAX_INTERACTION_DISTANCE);
    }
}
