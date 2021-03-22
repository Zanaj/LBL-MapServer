using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PlayerMovementUpdate : Packet
{
    public int characterID;
    public float x;
    public float y;
    public float z;
    public float rotX;
    public float rotY;
    public float rotZ;
    public PlayerMovementUpdate() { type = PacketType.PlayerMovementUpdate; }

    public override void Serialize()
    {
        BeginWrite();
        writer.Write(characterID);
        writer.Write(x);
        writer.Write(y);
        writer.Write(z);
        writer.Write(rotX);
        writer.Write(rotY);
        writer.Write(rotZ);
        EndWrite();
    }
}
