using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InGameUI : MonoBehaviour
{
    public ClientBehaviour client;
    public TMP_Text readyText;
    public Button readyButton;

    private void Start()
    {
        client = FindObjectOfType<ClientBehaviour>();
        UpdateReadyAmount(0);
    }

    public void ResetButton()
    {
        UpdateReadyAmount(0);
        readyButton.interactable = true;
    }

    public void UpdateReadyAmount(int readyPlayers)
    {
        readyText.text = readyPlayers.ToString() + "/" + PlayActionPlan.playerCount.ToString();
    }

    public void Ready()
    {
        readyButton.interactable = false;
        PlayActionPlan.ready = true;
        client.SendServerRequest("3");
    }
}
