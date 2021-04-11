using System.Collections;
using System.Collections.Generic;
using Telepathy;
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

    public override void OnRecieve(Message msg)
    {
        NetworkManager.client.Disconnect();
        NetworkManager.server.Start(NetworkManager.instance.port);
    }
}
