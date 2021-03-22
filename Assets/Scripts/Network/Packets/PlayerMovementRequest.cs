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
    public float x;
    public float z;

    public float rotX;
    public float rotY;
    public float rotZ;

    public PlayerMovementRequest() { type = PacketType.PlayerMovementRequest; }

    public override void Deserialize(BinaryReader reader)
    {
        x = reader.ReadSingle();
        z = reader.ReadSingle();

        rotX = reader.ReadSingle();
        rotY = reader.ReadSingle();
        rotZ = reader.ReadSingle();
    }

    public override void OnRecieve(Message msg)
    {
        int accountID = NetworkManager.instance.connectionToAccountID[msg.connectionId];

        ////TODO: check Collision map.
        Player updatedPlayer = PlayerManager.instance.onlinePlayers.Find(p => p.characterID == accountID);
        if (updatedPlayer != null)
        {
            updatedPlayer.transform.rotation = Quaternion.Euler(new Vector3(rotX, rotY, rotZ));
            Vector3 newPos = updatedPlayer.UpdatePos(x,z);
            
            PlayerMovementUpdate update = new PlayerMovementUpdate();
            update.characterID = accountID;
            update.x = newPos.x;
            update.y = newPos.y;
            update.z = newPos.z;
            update.rotX = rotX;
            update.rotY = rotY;
            update.rotZ = rotZ;
            
            update.Serialize();
            NetworkManager.instance.SendAll(update);
        }
    }
}
