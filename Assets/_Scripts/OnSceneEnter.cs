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
                military.SetActive(true);
                break;
            case 2:
                military.SetActive(true);
                terrorists.SetActive(true);
                break;
            case 3:
                military.SetActive(true);
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
