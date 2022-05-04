using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class PlayActionPlan : MonoBehaviour
{
    [SerializeField] public List<Unit> allUnits = new List<Unit>();

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
    }
}
