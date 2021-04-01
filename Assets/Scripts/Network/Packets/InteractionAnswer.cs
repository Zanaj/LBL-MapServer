using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionAnswer : Packet
{
    public bool isAllowed;
    public string entityGUID;

    public InteractionAnswer() { type = PacketType.InteractionAnswer; }

    public override void Serialize()
    {
        BeginWrite();
        writer.Write(isAllowed);
        writer.Write(entityGUID);
        EndWrite();
    }
}
