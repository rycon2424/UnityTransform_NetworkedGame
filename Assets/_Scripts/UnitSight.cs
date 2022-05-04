using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSight : MonoBehaviour
{
    public List<Unit> targetList = new List<Unit>();

    private NetworkedPlayer player;

    private void Start()
    {
        player = FindObjectOfType<NetworkedPlayer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Unit"))
        {
            Unit unit = other.GetComponent<Unit>();
            if (unit.idOwner != player.idOwner)
            {
                targetList.Add(unit);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Unit"))
        {
            Unit unit = other.GetComponent<Unit>();
            if (unit.idOwner != player.idOwner)
            {
                targetList.Remove(unit);
            }
        }
    }
}
