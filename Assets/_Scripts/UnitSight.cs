using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSight : MonoBehaviour
{
    public List<Unit> targetList = new List<Unit>();
    public Unit thisUnit;

    private void Start()
    {
        targetList.AddRange(FindObjectsOfType<Unit>());

        List<Unit> friendlies = new List<Unit>();

        foreach (Unit unit in targetList)
        {
            if (unit.idOwner == thisUnit.idOwner)
            {
                friendlies.Add(unit);
            }
        }
        foreach (Unit unit in friendlies)
        {
            targetList.Remove(unit);
        }
    }
}
