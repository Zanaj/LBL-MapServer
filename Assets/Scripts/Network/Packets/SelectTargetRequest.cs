using System.Collections;
using System.Collections.Generic;
using System.IO;
using Telepathy;
using UnityEngine;

public class SelectTargetRequest : Packet
{
    
    public string entityGUID;

    public SelectTargetRequest() { type = PacketType.SelectTargetRequest; }

    public override void Deserialize(BinaryReader reader)
    {
        entityGUID = reader.ReadString();
    }

    public override void OnRecieve(Message msg)
    {
        bool isAllowed = false;
        Entity entity = EntityManager.instance.entities.Find(x => x.entityGUID == entityGUID);
        if(entity != null)
        {
            int characterID = NetworkManager.instance.connectionToAccountID[msg.connectionId];
            Player requester = PlayerManager.instance.onlinePlayers.Find(x => x.characterID == characterID);

            Vector3 playerPos = requester.transform.position;
            Vector3 entityPos = entity.transform.position;

            float dis = Vector3.Distance(playerPos, entityPos);
            if(entity.type == EntityType.Enemy)
            {
                isAllowed = dis <= Player.TARGET_VIEW_DISTANCE;
            }
            else
            {
                isAllowed = dis <= NetworkManager.instance.MAX_INTERACTION_DISTANCE;
            }

            if (isAllowed)
            {
                isAllowed = entity.isInteractable && entity.options.Length > 0;
                if (isAllowed)
                {
                    requester.target = entity;
                }
            }
        }

            

        SelectTargetAnswer answer = new SelectTargetAnswer();
        answer.isAllowed = isAllowed;

        string id = entity == null ? "NA" : entity.entityGUID;
        answer.entityGUID = id;
        answer.Serialize();

        NetworkManager.Send(msg.connectionId, answer);
    }
}
