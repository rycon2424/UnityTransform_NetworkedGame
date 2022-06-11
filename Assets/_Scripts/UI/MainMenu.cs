using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

public class MainMenu : MonoBehaviour
{
    [ReadOnly][ShowInInspector] public static string username;
    [SerializeField] int playersToPlay = 2;
    [Space]
    [SerializeField] TMP_Text playerName;
    public TMP_InputField ipAdress;
    public TMP_InputField port;
    [Space]
    [SerializeField] TMP_Text playerCount;
    [SerializeField] TMP_Text playerNames;
    [Space]
    [SerializeField] Button startGameButton;
    [Space]
    [SerializeField] UnityEvent onGameStart;
    [Header("Lobby")]
    [SerializeField] GameObject[] players;

    public string GetPlayerName()
    {
        return playerName.text;
    }

    public void UpdateUsername()
    {
        username = playerName.text;
    }

    public void EnoughPlayersToStart(int players)
    {
        if (players > playersToPlay - 1)
        {
            startGameButton.interactable = true;
        }
        else
        {
            startGameButton.interactable = false;
        }
    }

    public void UpdatePlayerCount(int players)
    {
        PlayActionPlan.playerCount = players;
        playerCount.text = "Players = " + players.ToString() + "/3";
        UpdatePlayerVisibility(players);
    }

    void UpdatePlayerVisibility(int pCount)
    {
        foreach (var p in players)
        {
            p.SetActive(false);
        }
        for (int i = 0; i < pCount; i++)
        {
            players[i].SetActive(true);
        }
    }
    
    public void UpdatePlayerList(List<string> names)
    {
        string players = "";
        foreach (string pName in names)
        {
            players += pName + System.Environment.NewLine;
        }
        playerNames.text = players;
    }

    public void StartGame()
    {
        onGameStart.Invoke();
    }
}