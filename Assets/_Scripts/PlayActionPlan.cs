using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using TMPro;

public class PlayActionPlan : MonoBehaviour
{
    [SerializeField] public List<Unit> allUnits = new List<Unit>();
    [ReadOnly] [ShowInInspector] public static bool sequencing;
    [ReadOnly] [ShowInInspector] public static bool ready;
    [ReadOnly] [ShowInInspector] public static int playerCount;
    [Space]
    public UploadNewScore scoreUploader;
    public UnityEvent OnSequenceEnd;
    [Space]
    public GameObject gameOver;
    public TMP_Text gameOverText;

    private NetworkedPlayer player;

    void Awake()
    {
        player = FindObjectOfType<NetworkedPlayer>();
    }

    public void FindUnits()
    {
        allUnits.AddRange(FindObjectsOfType<Unit>());
    }

    [Button]
    void UpdateUnits()
    {
        allUnits.AddRange(FindObjectsOfType<Unit>());
        int id = 1;
        foreach (Unit u in allUnits)
        {
            u.unitID = id++;
            u.HideCharacter();
        }
    }

    public void UpdateUnit(int id, Vector3 position, int action)
    {
        foreach (var unit in allUnits)
        {
            if (unit.unitID == id)
            {
                PlayerAction actionType = PlayerAction.walk;
                actionType = (PlayerAction)action;
                Action actionToAdd = new Action(position, actionType);
                unit.plan.Add(actionToAdd);
                return;
            }
        }
        Debug.Log("Found no unit with ID " + id);
    }

    public void StartSequence()
    {
        player.DeSelectUnit();
        StartCoroutine(Sequence());
    }

    IEnumerator Sequence()
    {
        sequencing = true;
        foreach (Unit u in allUnits)
        {
            u.HideCharacter();
            u.StartPlan();
        }
        yield return new WaitForEndOfFrame();
        foreach (Unit u in allUnits)
        {
            u.RemoveLinePath();
            u.UnFreezeUnit();
        }
        VisualIconsPool.instance.ResetAllPools();
        while (EveryUnitCompletedPlan() == false)
        {
            yield return new WaitForSeconds(0.5f);
            foreach (Unit u in allUnits)
            {
                if (u.dead == false)
                {
                    if (u.targetTransform)
                    {
                        u.RevealCharacter();
                        u.Shoot();
                    }
                    else
                    {
                        u.HideCharacter();
                    }
                }
            }
            yield return new WaitForSeconds(0.5f);
        }
        foreach (Unit u in allUnits)
        {
            if (u.dead == false)
            {
                if (u.targetTransform)
                {
                    u.Shoot();
                    u.RevealCharacter();
                }
                else
                {
                    u.HideCharacter();
                }
            }
        }
        yield return new WaitForSeconds(1f);
        int winner = -1;
        if (CheckIfGameOver(out winner))
        {
            GameOver(winner);
        }
        else
        {
            foreach (Unit u in allUnits)
            {
                u.FreezeUnit();
            }
            foreach (Unit u in allUnits)
            {
                u.currentPoints = u.maxPoints;
            }
            ready = false;
            sequencing = false;
            OnSequenceEnd.Invoke();
        }
    }

    bool CheckIfGameOver(out int winner)
    {
        int aliveOwnerID = -1;
        foreach (Unit u in allUnits)
        {
            if (u.dead == false)
            {
                if (aliveOwnerID == -1)
                {
                    aliveOwnerID = u.idOwner;
                }
                else if (aliveOwnerID != u.idOwner)
                {
                    winner = -1;
                    return false;
                }
            }
        }
        winner = aliveOwnerID;
        return true;
    }

    bool EveryUnitCompletedPlan()
    {
        foreach (Unit u in allUnits)
        {
            if (u.plan.Count > 0)
            {
                return false;
            }
        }
        return true;
    }

    public void GameOver(int winnerID)
    {
        switch (winnerID)
        {
            case 1:
                gameOverText.text = "The Military Won!";
                break;
            case 2:
                gameOverText.text = "The Terrorists Won!";
                break;
            case 3:
                gameOverText.text = "The Special Forces Won!";
                break;
            default:
                break;
        }
        scoreUploader.UploadScore(UploadNewScore.currentPoints);
        gameOver.SetActive(true);
        Invoke("RestartGame", 5);
    }

    void RestartGame()
    {
        SceneManager.UnloadSceneAsync("FirstMap");
        SceneManager.LoadScene("MainMenu");
    }
}
