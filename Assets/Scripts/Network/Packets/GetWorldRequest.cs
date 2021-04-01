using System.Collections;
using System.Collections.Generic;
using System.IO;
using Telepathy;
using UnityEngine;

public class GetWorldRequest : Packet
{
    public int characterID;

    public GetWorldRequest() { type = PacketType.GetWorldRequest; }

    public override void Deserialize(BinaryReader reader)
    {
        characterID = reader.ReadInt32();
    }

    public override void OnRecieve(Message msg)
    {
        NetworkManager.instance.connectionToAccountID[msg.connectionId] = characterID;
        PlayerManager.instance.PlayerConnected(characterID);

        GetWorldAnswer answer = new GetWorldAnswer();
        answer.Serialize();
        NetworkManager.Send(msg.connectionId, answer);
    }
}
