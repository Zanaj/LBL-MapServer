using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegisterServer : Packet
{
    public int port;

    public RegisterServer() { type = PacketType.RegisterServer; }

    public override void Serialize()
    {
        BeginWrite();
        writer.Write(port);
        EndWrite();
    }
}
