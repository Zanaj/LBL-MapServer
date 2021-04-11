using System.Collections;
using System.Collections.Generic;
using System.IO;
using Telepathy;
using UnityEngine;

public class InteractionRequest : Packet
{
    public string entityGUID;
    public int option;
    public List<string> extraInfo;
    public InteractionRequest() { type = PacketType.InteractionRequest; }

    public override void Deserialize(BinaryReader reader)
    {
        entityGUID = reader.ReadString();
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

        Debug.Log($"{requester == null},{extraInfo == null}");

        InteractionErrorCode errorCode = EntityManager.instance
            .CanUseInteraction(requester, entityGUID, option, extraInfo);

        isAllowed = errorCode == InteractionErrorCode.Success;

        if (!isAllowed)
            Debug.Log($"Interaction Error: {errorCode} option: {option}");

        InteractionAnswer answer = new InteractionAnswer();
        answer.isAllowed = isAllowed;

        if (entityGUID == "NA")
            answer.entityGUID = requester.target.entityGUID;
        else
            answer.entityGUID = entityGUID;

        answer.Serialize();

        NetworkManager.Send(msg.connectionId, answer);

        if (isAllowed)
            EntityManager.instance.UseInteraction(requester, entityGUID, option, extraInfo);
    }
}
