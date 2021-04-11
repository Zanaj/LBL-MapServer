using System.IO;
using Telepathy;

/// <summary>
/// Base class for a packet of data over the network.
/// 
/// </summary>
public class Packet
{
    //The conneciton ID of the sender
    public int id = -1; 

    //The type of packet recieved.
    public PacketType type; 

    //The buffer of data of the packet to send. Use BeginWrite() and EndWrite() to write to this.
    public byte[] buffer; 

    //the writer so we can write. Make sure you call BeginWrite()
    //TODO: make this private and make custom functions so we can do checks if beginWrite is called prior to writing
    public BinaryWriter writer;
    
    //The memory stream for writing
    private MemoryStream stream;
    
    public Packet() { }
    public Packet(byte[] data)
    {
        buffer = data;
    }

    //Call this before writing to the buffer
    public void BeginWrite()
    {
        stream = new MemoryStream();
        writer = new BinaryWriter(stream);

        writer.Write((int)type);
        writer.Write(id);
    }

    //Call this when you're done writing to the buffer
    public void EndWrite()
    {
        //TODO: actually read up on this shit and close stuff properably if it aint.
        buffer = stream.ToArray();
        stream.Close();
    }

    //Overridable function to easy serialization
    public virtual void Serialize() { }

    //Overridable function for easy deserialization
    public virtual void Deserialize(BinaryReader reader) { }

    //This is called when this packet is recieved.
    public virtual void OnRecieve(Message msg) { }
}
