using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class PlayActionPlan : MonoBehaviour
{
    [SerializeField] public List<Unit> allUnits = new List<Unit>();
    public bool sequencing;

    void Start()
    {
        allUnits.AddRange(FindObjectsOfType<Unit>());
    }

    [Button]
    void PerformPlan()
    {
        foreach (Unit u in allUnits)
        {
            u.StartPlan();
        }
        StartCoroutine(Sequence());
    }

    IEnumerator Sequence()
    {
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
        foreach (Unit u in allUnits)
        {
            u.FreezeUnit();
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
