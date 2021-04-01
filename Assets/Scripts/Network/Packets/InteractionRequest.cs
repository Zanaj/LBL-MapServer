using System.Collections;
using System.Collections.Generic;
using System.IO;
using Telepathy;
using UnityEngine;

public class InteractionRequest : Packet
{
    public int option;
    public List<string> extraInfo;
    public InteractionRequest() { type = PacketType.InteractionRequest; }

    public override void Deserialize(BinaryReader reader)
    {
        option = reader.ReadInt32();
        int size = reader.ReadInt32();

        extraInfo = new List<string>();
        if (size > 0)
        {
            for (int i = 0; i < size; i++)
            {
                extraInfo.Add(reader.ReadString());
            }
        }
    }

    public override void OnRecieve(Message msg)
    {
        bool isAllowed = false;
        int characterID = NetworkManager.instance.connectionToAccountID[msg.connectionId];
        Player requester = PlayerManager.instance.onlinePlayers.Find(x => x.characterID == characterID);

        isAllowed = EntityManager.instance.CanUseInteraction(requester, option, extraInfo);

        InteractionAnswer answer = new InteractionAnswer();
        answer.isAllowed = isAllowed;
        answer.entityGUID = requester.target.entityGUID;
        answer.Serialize();

        NetworkManager.Send(msg.connectionId, answer);

        if (isAllowed)
            EntityManager.instance.UseInteraction(requester, option, extraInfo);
    }
}
