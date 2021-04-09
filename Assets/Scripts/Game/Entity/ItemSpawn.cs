using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemSpawn : Entity, IInteractable
{
    public override bool isInteractable => true;
    public override EntityType type => EntityType.Interactable;

    public Player owner;

    public ItemData item;
    public int amount;

    public override void Respawn() => Sync();
    private void Start()
    {
        EntityManager.instance.unclaimed.Add(this);
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
        if (!(activator is Player))
            return false;

        Player player = (Player)activator;


        return true;
    }

    public void InteractWith(Entity activator, List<string> extraInfo)
    {
        throw new System.NotImplementedException();
    }
}
