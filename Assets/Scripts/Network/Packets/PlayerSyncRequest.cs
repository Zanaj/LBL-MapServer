using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telepathy;

public class PlayerSyncRequest : Packet
{
    public int characterID;

    public PlayerSyncRequest() { type = PacketType.PlayerSyncRequest; }

    public override void Deserialize(BinaryReader reader)
    {
        characterID = reader.ReadInt32();
    }

    public override void OnRecieve(Message msg)
    {
        //TODO: move this to networkmanager messageloop on connection
        if (!NetworkManager.instance.connectionToAccountID.ContainsKey(msg.connectionId))
        {
            NetworkManager.instance.connectionToAccountID.Add(msg.connectionId, characterID);
            PlayerManager.instance.PlayerConnected(characterID);
        }

        Player[] onlinePlayers = PlayerManager.instance.onlinePlayers.ToArray();
        for (int i = 0; i < onlinePlayers.Length; i++)
        {
            Player acc = onlinePlayers[i];

            PlayerSyncAnswer answer = new PlayerSyncAnswer()
            {
                name = acc.name,
                genativPronoun = acc.genativ,
                referalPronoun = acc.referal,

                bodyType = acc.bodyType,
                height = acc.height,
                weight = acc.weight,

                targetX = acc.transform.position.x,
                targetY = acc.transform.position.y,
                targetZ = acc.transform.position.z,

                characterID = acc.characterID
            };

            answer.Serialize();
            NetworkManager.instance.SendAll(answer);
        }
    }
}
