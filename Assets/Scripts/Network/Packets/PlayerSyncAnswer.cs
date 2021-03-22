using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PlayerSyncAnswer : Packet
{
    public string name;
    public string genativPronoun;
    public string referalPronoun;

    public int bodyType;

    public float height;
    public float weight;

    public float targetX;
    public float targetY;
    public float targetZ;

    public int characterID;

    public PlayerSyncAnswer()
    {
        type = PacketType.PlayerSyncAnswer;
    }

    public override void Serialize()
    {
        BeginWrite();
        writer.Write(name);
        writer.Write(genativPronoun);
        writer.Write(referalPronoun);

        writer.Write(bodyType);

        writer.Write(height);
        writer.Write(weight);

        writer.Write(targetX);
        writer.Write(targetY);
        writer.Write(targetZ);

        writer.Write(characterID);
        EndWrite();
    }
}
