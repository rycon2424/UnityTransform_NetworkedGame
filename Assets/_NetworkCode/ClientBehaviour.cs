using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using Sirenix.OdinInspector;
using TMPro;

public class ClientBehaviour : MonoBehaviour
{
    [Sirenix.OdinInspector.ReadOnly] public NetworkedPlayer player;
    [Sirenix.OdinInspector.ReadOnly] public PlayActionPlan sequencer;
    public MainMenu mainMenu;
    [Space]
    public NetworkDriver m_Driver;
    public NetworkConnection m_Connection;

    private bool creatingConnection;

    private void Start()
    {
        player = FindObjectOfType<NetworkedPlayer>();
        sequencer = FindObjectOfType<PlayActionPlan>();
    }

    public void StartClient()
    {
        m_Driver = NetworkDriver.Create();
        m_Connection = default(NetworkConnection);

        var endpoint = NetworkEndPoint.LoopbackIpv4;
        endpoint.Port = 9000;
        m_Connection = m_Driver.Connect(endpoint);

        creatingConnection = true;
    }

    public void Disconnect()
    {
        m_Connection.Disconnect(m_Driver);
        m_Connection = default(NetworkConnection);
    }

    [Button]
    public void GetID() // Calls when game starts
    {
        SendServerRequest("0");
    }

    public void SendServerRequest(string request)
    {
        string serverRequest = request;
        m_Driver.BeginSend(m_Connection, out var writer);
        writer.WriteFixedString128(serverRequest);
        m_Driver.EndSend(writer);
    }

    public void OnDestroy()
    {
        m_Driver.Dispose();
    }

    void Update()
    {
        if (!creatingConnection)
            return;

        m_Driver.ScheduleUpdate().Complete();
        if (!m_Connection.IsCreated)
        {
            Debug.Log("Something went wrong during connect");
            return;
        }

        DataStreamReader stream;
        NetworkEvent.Type cmd;
        while ((cmd = m_Connection.PopEvent(m_Driver, out stream)) != NetworkEvent.Type.Empty)
        {
            switch (cmd)
            {
                case NetworkEvent.Type.Empty:
                    Debug.Log("Connecting?3");
                    break;

                case NetworkEvent.Type.Data:
                    var dataReceived = stream.ReadFixedString128();
                    ConvertData(dataReceived);
                    break;

                case NetworkEvent.Type.Connect:
                    SendServerRequest("4 " + mainMenu.GetPlayerName());
                    Debug.Log("We are now connected to the server");
                    break;

                case NetworkEvent.Type.Disconnect:
                    Debug.Log("Client got disconnected from server");
                    m_Connection = default(NetworkConnection);
                    break;

                default:
                    break;
            }
        }
    }

    public void ConvertData(FixedString128Bytes input)
    {
        List<float> parsedBytes = NetworkMessageHandler.GetDigits(input);
        // [0] What to do 
        // 0 [1]=ID Received
        // 2 [1]=UnitID [2]=ActionType [3]=PositionX [4]=PositionY [5]PositionZ
        // 3 [1]=ID [2] 1=Ready 2=not Ready
        // 4 Ask server for ID
        // 5 Update player count
        // 6 Update player list
        switch (parsedBytes[0])
        {
            case 0: // Received ID
                player.idOwner = (int)parsedBytes[1];
                player.ReceivedID();
                break;
            case 1:
                break;
            case 2: // Update a Players Units
                Vector3 pos = new Vector3(parsedBytes[3], parsedBytes[4], parsedBytes[5]);
                sequencer.UpdateUnit((int)parsedBytes[1], pos, (int)parsedBytes[2]);
                break;
            case 3: // a Player is Ready
                break;
            case 4: // Ask server for ID
                GetID();
                mainMenu.StartGame();
                break;
            case 5: // UpdatePlayerCount
                mainMenu.UpdatePlayerCount((int)parsedBytes[1]);
                break;
            case 6: // UpdatePlayerList
                mainMenu.UpdatePlayerList(NetworkMessageHandler.GetOnlyCharacters(input));
                break;
            default:
                Debug.Log($"Client does not know what to do with {input}");
                break;
        }
    }
}