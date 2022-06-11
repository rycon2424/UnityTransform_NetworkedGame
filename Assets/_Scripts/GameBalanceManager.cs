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
    public int grenadeCost = 15;
    [Space]
    public int costWalkUnitExtra = 2;
    public int costRunUnitExtra = 1;
    public int costGrenadeUnitExtra = 3;
    [Space]
    public int distanceTillCost = 8;
    [Space]
    public float grenadeRange = 3;
    public int grenadeDamage = 25;

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