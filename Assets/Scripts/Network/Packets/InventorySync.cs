using System.Collections;
using System.Collections.Generic;
using System.IO;
using Telepathy;
using UnityEngine;

public class InventorySync : Packet
{
    public Player player;
    public InventorySync() => type = PacketType.InventorySync;

    public override void Serialize()
    {
        BeginWrite();
        writer.Write(player.inventory.Count);
        if(player.inventory.Count > 0)
        {
            for (int i = 0; i < player.inventory.Count; i++)
            {
                ItemEntry item = player.inventory[i];
                writer.Write(item.itemID);
                writer.Write(item.amount);
            }
        }
        EndWrite();
    }

}
