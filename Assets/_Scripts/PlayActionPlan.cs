using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class PlayActionPlan : MonoBehaviour
{
    [SerializeField] public List<Unit> allUnits = new List<Unit>();
    [ReadOnly] [ShowInInspector] public static bool sequencing;

    private NetworkedPlayer player;

    void Start()
    {
        player = FindObjectOfType<NetworkedPlayer>();
        allUnits.AddRange(FindObjectsOfType<Unit>());
    }

    [Button]
    void PerformPlan()
    {
        player.DeSelectUnit();
        StartCoroutine(Sequence());
    }

    IEnumerator Sequence()
    {
        // Get and Send Plan online
        foreach (Unit u in allUnits)
        {
            u.StartPlan();
        }
        yield return new WaitForEndOfFrame();
        sequencing = true;
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
        sequencing = false;
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
