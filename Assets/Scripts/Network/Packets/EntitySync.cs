    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class EntitySync : Packet
{
    public EntityType entityType;
    public Entity entity;
    
    public EntitySync()
    {
        type = PacketType.EntitySync;
    }

    public override void Serialize()
    {
        BeginWrite();
        writer.Write((int)entityType);
        entity.MakeEntityPacket(writer);
        EndWrite();
    }
}
