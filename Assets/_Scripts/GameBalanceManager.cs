using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class GameBalanceManager : MonoBehaviour
{
    public int maxPoints = 40;
    public int unitsPerTeam = 4;
    [Space]
    public int walkCost = 15;
    public int runCost = 10;
    public int lookCost = 5;
    [Space]
    public int costWalkUnitExtra = 2;
    public int costRunUnitExtra = 1;
    [Space]
    public int distanceTillCost = 8;

    public static GameBalanceManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
    }
}