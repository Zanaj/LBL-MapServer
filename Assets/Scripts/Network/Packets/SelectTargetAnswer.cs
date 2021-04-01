using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectTargetAnswer : Packet
{
    public bool isAllowed;
    public string entityGUID;
    public SelectTargetAnswer() { type = PacketType.SelectTargetAnswer; }

    public override void Serialize()
    {
        BeginWrite();
        writer.Write(isAllowed);
        writer.Write(entityGUID);
        EndWrite();
    }
}
