using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class PlayActionPlan : MonoBehaviour
{
    [SerializeField] public List<Unit> allUnits = new List<Unit>();
    [ReadOnly] [ShowInInspector] public static bool sequencing;
    [ReadOnly] [ShowInInspector] public static bool ready;
    [ReadOnly] [ShowInInspector] public static int playerCount;
    [Space]
    public UnityEvent OnSequenceEnd;

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
                switch (action)
                {
                    case 0:
                        actionType = PlayerAction.walk;
                        break;
                    case 1:
                        actionType = PlayerAction.run;
                        break;
                    case 2:
                        actionType = PlayerAction.look;
                        break;
                    default:
                        break;
                }
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
        VisualIconsPool.instance.ResetAllEyes();
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
            Debug.Log("winner = " + winner);
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
}
