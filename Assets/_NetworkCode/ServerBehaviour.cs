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
    public int playersReady = 0;
    public int maxAllowedPlayers = 3;
    [Space]
    public MainMenu mainMenu;
    public List<string> players = new List<string>();

    private bool creatingConnection;
    private bool gameStarted;

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
        mainMenu.EnoughPlayersToStart(m_Connections.Length);
    }

    void AcceptNewConnections()
    {
        if (gameStarted)
        {
            return;
        }
        if (m_Connections.Length == maxAllowedPlayers)
        {
            return;
        }
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

    public void UpdatePlayerCount()
    {

    }

    public void StartGame()
    {
        gameStarted = true;
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
        List<float> parsedBytes = NetworkMessageHandler.GetDigits(input);
        // [0] What to do 
        // 1 [1]=ID
        // 2 [1]=UnitID [2]=ActionType [3]=PositionX [4]=PositionY [5]PositionZ
        // 3 [1]=ID [2] 1=Ready 0=not Ready
        // 4 Send Updated Playerlist

        switch (parsedBytes[0])
        {
            case 0: // Request ID
                SendID(sender);
                break;
            case 1: // Disconnect
                break;
            case 2: // Update Units and send to all except player who send
                Debug.Log("Server received unit Update");
                SendToAll
                    (
                    "2 "
                    + parsedBytes[1] + " "
                    + parsedBytes[2] + " " 
                    + parsedBytes[3] + " " 
                    + parsedBytes[4] + " " 
                    + parsedBytes[5]
                    );
                break;
            case 3: // a Player is Ready
                playersReady++;
                if (playersReady == PlayActionPlan.playerCount)
                {
                    SendToAll("7");
                    playersReady = 0;
                }
                else
                {
                    SendToAll("8 " + playersReady);
                }
                break;
            case 4:
                players.Add(input.ToString());
                string playerNamesList = "6 ";
                foreach (string pName in players)
                {
                    playerNamesList += pName + " ";
                }
                SendToAll(playerNamesList);
                break;
            default:
                Debug.Log($"Server does not know what to do with {input}");
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
    public static List<float> GetDigits(FixedString128Bytes input)
    {
        string phrase = input.ToString();
        string[] words = phrase.Split(' ');
        List<float> parsedNumbers = new List<float>();
        float parsedWord = 0;
        foreach (var word in words)
        {
            bool getDigit = float.TryParse(word, out parsedWord);
            if (getDigit)
            {
                parsedNumbers.Add(parsedWord);
            }
        }
        return parsedNumbers;
    }

    public static List<string> GetOnlyCharacters(FixedString128Bytes input)
    {
        string phrase = input.ToString();
        string[] words = phrase.Split(' ');
        List<string> allValues = new List<string>();
        foreach (var word in words)
        {
            allValues.Add(word);
        }
        List<string> onlyCharacterWords = new List<string>();
        int temp = 0;
        foreach (var word in allValues)
        {
            bool isChar = int.TryParse(word, out temp);
            if (isChar == false)
            {
                onlyCharacterWords.Add(word);
            }
        }
        return onlyCharacterWords;
    }
}