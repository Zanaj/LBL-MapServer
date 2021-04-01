using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telepathy;
using UnityEngine;

public class PlayerMovementRequest : Packet
{
    public float look;
    public List<bool> inputs;

    public PlayerMovementRequest() { type = PacketType.PlayerMovementRequest; }

    public override void Deserialize(BinaryReader reader)
    {
        look = reader.ReadSingle();
        inputs = new List<bool>();
        int size = reader.ReadInt32();
        for (int i = 0; i < size; i++)
        {
            inputs.Add(reader.ReadBoolean());
        }
    }

    public override void OnRecieve(Message msg)
    {
        int characterID = NetworkManager.instance.connectionToAccountID[msg.connectionId];
        Player player = PlayerManager.instance.onlinePlayers.Find(x => x.characterID == characterID);
        player.inputs = inputs.ToArray();
        player.rotation = look;
    }
}
