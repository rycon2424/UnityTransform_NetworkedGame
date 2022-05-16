using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using TMPro;

public class UISelection : MonoBehaviour
{
    [ReadOnly] public bool canSelect = true;
    [ReadOnly] public bool menuOpen;
    [Space]
    [SerializeField] bool showReferences;
    [SerializeField] bool showPrivateValues;
    [Space]
    [ShowIf("@showReferences")] [SerializeField] Transform lookat;
    [ShowIf("@showReferences")] [SerializeField] GameObject menu;
    [Space]
    [ShowIf("@showReferences")][SerializeField] TMP_Text points;
    [Space]
    [ShowIf("@showReferences")] [SerializeField] ActionButton walkCost;
    [ShowIf("@showReferences")] [SerializeField] ActionButton runCost;
    [ShowIf("@showReferences")] [SerializeField] ActionButton lookCost;
    [Space]
    [SerializeField] NetworkedPlayer player;
    [Space]
    [ShowIf("@showPrivateValues")][SerializeField] int currentWalkCost;
    [ShowIf("@showPrivateValues")][SerializeField] int currentRunCost;
    [ShowIf("@showPrivateValues")][SerializeField] int currentLookCost;
    [Space]
    [ShowIf("@showPrivateValues")][SerializeField] Vector3 currentVirtualPosition;

    void Start()
    {
        currentWalkCost = GameBalanceManager.instance.walkCost;
        currentRunCost = GameBalanceManager.instance.runCost;
        currentLookCost = GameBalanceManager.instance.lookCost;
        player = FindObjectOfType<NetworkedPlayer>();
    }

    public void ShowOptions(Vector3 pos)
    {
        UpdatePlayerVirtualLocation();
        menuOpen = true;
        menu.SetActive(true);
        transform.position = pos;
        CalculateCostOnDistance();
        UpdateUI();
    }

    void CalculateCostOnDistance()
    {
        GetCost();

        Vector3 unitPos = currentVirtualPosition;
        Vector3 selectionPos = transform.position;

        float distance = Vector3.Distance(unitPos, selectionPos);
        if (distance > GameBalanceManager.instance.distanceTillCost)
        {
            int remaining = Mathf.RoundToInt(distance - GameBalanceManager.instance.distanceTillCost);
            if (remaining > 0)
            {
                currentWalkCost += remaining * GameBalanceManager.instance.costWalkUnitExtra;
                currentRunCost += remaining * GameBalanceManager.instance.costRunUnitExtra;
            }
        }
    }

    public void UpdatePlayerVirtualLocation()
    {
        if (player.currentSelectedUnit.plan.Count < 1)
        {
            currentVirtualPosition = player.currentSelectedUnit.transform.position;
        }
        else
        {
            foreach (Action action in player.currentSelectedUnit.plan)
            {
                if (action.actionType != PlayerAction.look)
                {
                    currentVirtualPosition = action.targetLocation;
                }
            }
        }
    }

    public void UpdateUI()
    {
        int playerPoints = player.currentSelectedUnit.currentPoints;

        walkCost.cost.text = currentWalkCost.ToString();
        runCost.cost.text = currentRunCost.ToString();
        lookCost.cost.text = currentLookCost.ToString();

        walkCost.CanAfford(currentWalkCost, playerPoints);
        runCost.CanAfford(currentRunCost, playerPoints);
        lookCost.CanAfford(currentLookCost, playerPoints);

        points.text = $"{playerPoints} / {GameBalanceManager.instance.maxPoints}";
    }

    void GetCost()
    {
        currentWalkCost = GameBalanceManager.instance.walkCost;
        currentRunCost = GameBalanceManager.instance.runCost;
        currentLookCost = GameBalanceManager.instance.lookCost;
    }

    public void HideOptions()
    {
        menuOpen = false;
        menu.SetActive(false);
    }

    public void Button_Run()
    {
        player.currentSelectedUnit.currentPoints -= currentRunCost;
        SelectionMade(transform.position, PlayerAction.run);
    }

    public void Button_Walk()
    {
        player.currentSelectedUnit.currentPoints -= currentWalkCost;
        SelectionMade(transform.position, PlayerAction.walk);
    }

    public void Button_Look()
    {
        player.currentSelectedUnit.currentPoints -= currentLookCost;
        SelectionMade(transform.position, PlayerAction.look);
    }

    public void Button_Cancel()
    {
        HideOptions();
    }

    void SelectionMade(Vector3 pos, PlayerAction action)
    {
        player.SetChoice(pos, action, currentVirtualPosition);
        UpdatePlayerVirtualLocation();
        HideOptions();
    }

    void Update()
    {
        BoxLookAtCam();
    }

    void BoxLookAtCam()
    {
        Vector3 targetPostition = new Vector3
            (
            player.cam.transform.position.x,
            player.cam.transform.position.y,
            lookat.transform.position.z
            );
        lookat.transform.LookAt(targetPostition);
        lookat.transform.Rotate(180, 0, 180);
        if (lookat.transform.rotation.eulerAngles.y > 100)
        {
            lookat.transform.Rotate(0, 0, 180);
        }
    }
}

[System.Serializable]
public class ActionButton
{
    public TMP_Text cost;
    public Button button;

    public void CanAfford(int cost, int points)
    {
        if (cost > points)
        {
            button.interactable = false;
            return;
        }
        button.interactable = true;
    }
}