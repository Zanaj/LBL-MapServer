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
        int characterID = NetworkManager.instance.connectionToAccountID[msg.connectionId];
        Player requester = PlayerManager.instance.onlinePlayers.Find(x => x.characterID == characterID);

        if(requester.pendingTargetGUID == string.Empty)
        {
            requester.pendingTargetGUID = entityGUID;
            Entity targetEntity = EntityManager.instance.entities.Find(x => x.entityGUID == entityGUID);
            float maxDistance = 0;

            if(targetEntity != null)
            {
                switch (targetEntity.type)
                {
                    case EntityType.Unknown:
                        maxDistance = Player.TARGET_VIEW_DISTANCE;
                        break;
                    case EntityType.Interactable:
                        maxDistance = Player.TARGET_VIEW_DISTANCE;
                        break;
                    case EntityType.Enemy:
                        maxDistance = Player.TARGET_VIEW_DISTANCE;
                        break;
                    case EntityType.Player:
                        maxDistance = Player.TARGET_VIEW_DISTANCE;
                        break;
                    case EntityType.NPC:
                        maxDistance = Player.TARGET_VIEW_DISTANCE;
                        break;
                    case EntityType.Special:
                        maxDistance = Player.TARGET_VIEW_DISTANCE;
                        break;
                    default:
                        break;
                }

                Vector3 reqPos = requester.transform.position;
                Vector3 entPos = targetEntity.transform.position;

                float distance = Vector3.Distance(reqPos, entPos);
                if(distance <= maxDistance)
                {
                    if(targetEntity.options.Length > 0 && targetEntity.isInteractable)
                    {
                        isAllowed = true;
                        requester.target = targetEntity;
                    }
                }
            }
            else { Debug.Log("Target null with the ID of: " + entityGUID); }
        }
        else { Debug.Log("Still processing target"); }

        SelectTargetAnswer answer = new SelectTargetAnswer();
        answer.entityGUID = requester.pendingTargetGUID;
        answer.isAllowed = isAllowed;
        answer.Serialize();
        NetworkManager.Send(msg.connectionId, answer);

        requester.pendingTargetGUID = string.Empty;
    }
}
