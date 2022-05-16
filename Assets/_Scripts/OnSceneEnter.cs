using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnSceneEnter : MonoBehaviour
{
    void Start()
    {
        FindObjectOfType<PlayActionPlan>().FindUnits();
    }
}
