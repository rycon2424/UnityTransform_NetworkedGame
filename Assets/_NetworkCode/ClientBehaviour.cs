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
    public NetworkedPlayer player;
    public PlayActionPlan sequencer;
    public TMP_Text playerCount;
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

    [Button]
    public void GetID() // Calls when game starts
    {
        string serverReponse = "0";
        m_Driver.BeginSend(m_Connection, out var writer);
        writer.WriteFixedString128(serverReponse);
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
                    break;

                case NetworkEvent.Type.Data:
                    var dataReceived = stream.ReadFixedString128();
                    Debug.Log("Got the value = " + dataReceived + " back from the server");
                    ConvertData(dataReceived);
                    break;

                case NetworkEvent.Type.Connect:
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
        List<int> parsedBytes = NetworkMessageHandler.GetDigits(input);
        // [0] What to do 
        // 1 ID Received
        // 2 [1]=UnitID [2]=ActionType [3]=PositionX [4]=PositionY [5]PositionZ
        // 3 [1]=ID [2] 1=Ready 2=not Ready
        // 4 Ask server for ID

        switch (parsedBytes[0])
        {
            case 0: // Received ID
                player.idOwner = parsedBytes[1];
                break;
            case 1: // Disconnect
                // Show somewhere that player {ID} disconnected
                break;
            case 2: // Update a Players Units
                for (int i = 1; i < parsedBytes.Count; i+= 5)
                {
                    Vector3 pos = new Vector3(parsedBytes[i + 2], parsedBytes[i + 3], parsedBytes[i + 4]);
                    sequencer.UpdateUnit(parsedBytes[i], pos, parsedBytes [i + 1]);
                }
                break;
            case 3: // a Player is Ready
                break;
            case 4: // Ask server for ID
                GetID();
                break;
            case 5: // UpdatePlayerCount
                playerCount.text =  "Players = " + parsedBytes[1].ToString();
                break;
            default:
                break;
        }
    }
}