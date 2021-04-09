using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemSpawn : Entity
{
    public override bool isInteractable => true;
    public override EntityType type => EntityType.Interactable;

    public Player owner;

    public ItemBase item;
    public int amount;

    public override void Respawn() => Sync();

    private void Awake()
    {
        EntityManager.instance.unclaimed.Add(this);
    }

    private void Start()
    {
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
}
