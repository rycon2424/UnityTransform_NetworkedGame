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

    void Start()
    {
        player = FindObjectOfType<NetworkedPlayer>();
        allUnits.AddRange(FindObjectsOfType<Unit>());
        int id = 0;
        foreach (Unit u in allUnits) // Test if its the same in online
        {
            u.unitID = id++;
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

    [Button]
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
            u.StartPlan();
        }
        yield return new WaitForEndOfFrame();
        foreach (Unit u in allUnits)
        {
            u.RemoveLinePath();
            u.UnFreezeUnit();
        }
        while (EveryUnitCompletedPlan() == false)
        {
            yield return new WaitForSeconds(1f);
            foreach (Unit u in allUnits)
            {
                if (u.dead == false)
                {
                    if (u.targetTransform)
                    {
                        u.Shoot();
                    }
                }
            }
        }
        yield return new WaitForSeconds(1f);
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
