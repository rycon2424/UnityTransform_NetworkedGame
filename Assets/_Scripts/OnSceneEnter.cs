using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnSceneEnter : MonoBehaviour
{
    [SerializeField] GameObject military;
    [SerializeField] GameObject terrorists;
    [SerializeField] GameObject special;

    void Start()
    {
        switch (PlayActionPlan.playerCount)
        {
            case 1:
                break;
            case 2:
                terrorists.SetActive(true);
                break;
            case 3:
                terrorists.SetActive(true);
                special.SetActive(true);
                break;
            default:
                break;
        }

        FindObjectOfType<PlayActionPlan>().allUnits = new List<Unit>();
        FindObjectOfType<PlayActionPlan>().FindUnits();
    }
}
