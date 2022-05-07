using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [SerializeField] TMP_InputField playerName;
    [SerializeField] TMP_Text playerCount;
    [SerializeField] TMP_Text playerNames;
    [Space]
    [SerializeField] UnityEvent onGameStart;

    public string GetPlayerName()
    {
        return playerName.text;
    }

    public void UpdatePlayerCount(int players)
    {
        PlayActionPlan.playerCount = players;
        playerCount.text = "Players = " + players.ToString() + "/3";
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