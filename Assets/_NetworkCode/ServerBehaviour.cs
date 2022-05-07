using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;

public class ServerBehaviour : MonoBehaviour
{
    public NetworkDriver m_Driver;
    private NativeList<NetworkConnection> m_Connections;

    public int idToGive = 0;

    private bool creatingConnection;

    public void StartServer()
    {
        m_Driver = NetworkDriver.Create();
        var endpoint = NetworkEndPoint.AnyIpv4; // The local address to which the client will connect to is 127.0.0.1
        endpoint.Port = 9000;

        if (m_Driver.Bind(endpoint) != 0)
            Debug.Log("Failed to bind to port 9000");
        else
            m_Driver.Listen();

        m_Connections = new NativeList<NetworkConnection>(16, Allocator.Persistent);

        creatingConnection = true;
    }

    public void OnDestroy()
    {
        m_Driver.Dispose();
        m_Connections.Dispose();
    }

    void Update()
    {
        if (!creatingConnection)
            return;

        m_Driver.ScheduleUpdate().Complete();
        CleanUpConnections();
        AcceptNewConnections();
        RetrieveData();
    }

    void CleanUpConnections()
    {
        for (int i = 0; i < m_Connections.Length; i++)
        {
            if (!m_Connections[i].IsCreated)
            {
                m_Connections.RemoveAtSwapBack(i);
                --i;
            }
        }
    }

    void AcceptNewConnections()
    {
        NetworkConnection c;
        while ((c = m_Driver.Accept()) != default(NetworkConnection))
        {
            m_Connections.Add(c);
            Debug.Log("Accepted a connection");
            SendToAll("5 " + m_Connections.Length);
        }
    }

    void RetrieveData()
    {
        DataStreamReader stream;
        for (int i = 0; i < m_Connections.Length; i++)
        {
            NetworkEvent.Type cmd;
            while ((cmd = m_Driver.PopEventForConnection(m_Connections[i], out stream)) != NetworkEvent.Type.Empty)
            {
                if (cmd == NetworkEvent.Type.Data)
                {
                    var number = stream.ReadFixedString128();

                    ConvertData(number, m_Connections[i]);
                }
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    Debug.Log("Client disconnected from server");
                    m_Connections[i] = default(NetworkConnection);
                }
            }
        }
    }

    public void StartGame()
    {
        SendToAll("4");
    }

    void SendToAll(FixedString128Bytes number)
    {
        for (int i = 0; i < m_Connections.Length; i++)
        {
            m_Driver.BeginSend(NetworkPipeline.Null, m_Connections[i], out var writer);
            writer.WriteFixedString128(number);
            m_Driver.EndSend(writer);
        }
    }

    public void ConvertData(FixedString128Bytes input, NetworkConnection sender)
    {
        List<int> parsedByes = NetworkMessageHandler.GetDigits(input);
        // [0] What to do 
        // 1 [1]=ID
        // 2 [1]=UnitID [2]=ActionType [3]=PositionX [4]=PositionY [5]PositionZ

        // 3 [2] 1=Ready 2=not Ready

        switch (parsedByes[0])
        {
            case 0: // Request ID
                SendID(sender);
                break;
            case 1: // Disconnect
                break;
            case 2: // Update Units and send to all except player who send
                break;
            case 3: // a Player is Ready
                break;
            default:
                break;
        }
    }

    void SendID(NetworkConnection sender)
    {
        idToGive++;
        FixedString128Bytes idByte = "0 " + idToGive.ToString();
        m_Driver.BeginSend(NetworkPipeline.Null, sender, out var writer);
        writer.WriteFixedString128(idByte);
        m_Driver.EndSend(writer);
    }
}

public static class NetworkMessageHandler
{
    public static List<int> GetDigits(FixedString128Bytes input)
    {
        string phrase = input.ToString();
        string[] words = phrase.Split(' ');
        List<int> parsedNumbers = new List<int>();
        foreach (var word in words)
        {
            parsedNumbers.Add(int.Parse(word));
        }
        return parsedNumbers;
    }
}