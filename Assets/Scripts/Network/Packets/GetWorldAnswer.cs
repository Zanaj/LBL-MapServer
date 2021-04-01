using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetWorldAnswer : Packet
{
    public GetWorldAnswer() { type = PacketType.GetWorldAnswer; }

    public override void Serialize()
    {
        BeginWrite();
        int npcs = NPCManager.instance.NPCS.Count;
        writer.Write(npcs);
        if(npcs > 0)
        {
            for (int i = 0; i < NPCManager.instance.NPCS.Count; i++)
            {
                Entity npc = NPCManager.instance.NPCS[i];
                npc.MakeEntityPacket(writer);
            }
        }

        int enemies = EnemyManager.instance.enemies.Count;
        writer.Write(enemies);
        if (enemies > 0)
        {
            for (int i = 0; i < EnemyManager.instance.enemies.Count; i++)
            {
                Entity enemy = EnemyManager.instance.enemies[i];
                enemy.MakeEntityPacket(writer);
            }
        }

        int players = PlayerManager.instance.onlinePlayers.Count;
        writer.Write(players);
        if(players > 0)
        {
            for (int i = 0; i < PlayerManager.instance.onlinePlayers.Count; i++)
            {
                Player player = PlayerManager.instance.onlinePlayers[i];
                player.MakeEntityPacket(writer);
            }
        }
        EndWrite();
    }
}
