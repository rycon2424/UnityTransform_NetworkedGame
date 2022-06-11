using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnSceneEnter : MonoBehaviour
{
    void Start()
    {
        FindObjectOfType<PlayActionPlan>().allUnits = new List<Unit>();
        FindObjectOfType<PlayActionPlan>().FindUnits();
    }
}
