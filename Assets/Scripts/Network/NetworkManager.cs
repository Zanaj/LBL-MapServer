using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Telepathy;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum NetworkState
{
    NotConnected,
    Connecting,
    Connected,
    NotLoggedIn,
    StartingUp,
    Handshake,
    ProcessingLogin,
    CharacterScreen,
    LoggedIn,
}

public enum ConsoleLevel
{
    Minimal,
    Default,
    Verbose,
    Debug,
}

public class NetworkManager : MonoBehaviour
{
    [Header("Base Server Info")]
    public static NetworkManager instance;
    public GameObject playerPrefab;
    public int port;
    public Dictionary<int, int> connectionToAccountID;
    public static Server server = new Server();
    public bool isActive;

    private void Awake()
    {
        Telepathy.Logger.Log = Debug.Log;
        Telepathy.Logger.LogWarning = Debug.LogWarning;
        Telepathy.Logger.LogWarning = Debug.LogError;

        connectionToAccountID = new Dictionary<int, int>();
        
        instance = this;
        DontDestroyOnLoad(this);
        server.Start(port);
    }

    private void Update()
    {
        isActive = server.Active;
        MessageLoop();
    }

    private void MessageLoop()
    {
        if (server.Active)
        {
            Message msg;
            while (server.GetNextMessage(out msg))
            {
                if(msg.eventType == Telepathy.EventType.Connected)
                {
                    //TODO: proper loginRegistering?
                }
                else if(msg.eventType == Telepathy.EventType.Disconnected)
                {
                    PlayerManager.instance.PlayerDisconnected(msg.connectionId);
                }
                else if (msg.eventType == Telepathy.EventType.Data)
                {
                    MemoryStream stream;
                    BinaryReader reader;

                    stream = new MemoryStream(msg.data);
                    reader = new BinaryReader(stream);

                    PacketType type = (PacketType)reader.ReadInt32();
                    int id = reader.ReadInt32();

                    if(type != PacketType.PlayerMovementRequest)
                        Debug.Log("R: " + type);

                    Packet packet = GetPacketByType(type);
                    if (packet != null)
                    {
                        packet.Deserialize(reader);
                        packet.OnRecieve(msg);
                    }
                    else { Debug.LogWarning("Forgot implenetation for packet type: " + type); }
                }
            }
        }
    }

    private Packet GetPacketByType(PacketType type)
    {
        switch (type)
        {
            case PacketType.PlayerMovementRequest:
                return new PlayerMovementRequest();

            case PacketType.PlayerSyncRequest:
                return new PlayerSyncRequest();

            default:
                return null;
        }
    }

    public static void Send(int sendTo, Packet packet)
    {
        if (packet.type != PacketType.PlayerMovementUpdate)
        {
            Debug.Log("S: " + packet.type + " ID:" + sendTo);
        }

        server.Send(sendTo, packet.buffer);
    }

    public void SendAll(Packet packet)
    {
        foreach (var ids in connectionToAccountID)
        {
            Send(ids.Key, packet);
        }
    }
    public void SendAllExpect(int avoid, Packet packet)
    {
        foreach (var ids in connectionToAccountID)
        {
            if (ids.Value != avoid)
                Send(ids.Value, packet);
        }
    }


    private void OnApplicationQuit()
    {
        server.Stop();
    }
}
