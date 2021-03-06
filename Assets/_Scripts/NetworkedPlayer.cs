using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class NetworkedPlayer : NetworkedObject
{
    [Space]
    public Camera cam;
    [SerializeField] UISelection selection;
    [SerializeField] ClientBehaviour client;
    [ReadOnly] public Unit currentSelectedUnit;

    public void EnableUnits()
    {
        StartCoroutine(FindMyUnits());
    }

    IEnumerator FindMyUnits()
    {
        yield return new WaitForEndOfFrame();
        List<Unit> myUnits = new List<Unit>();
        myUnits.AddRange(FindObjectsOfType<Unit>());
        foreach (Unit unit in myUnits)
        {
            if (unit.idOwner == idOwner)
            {
                unit.isMine = true;
                unit.unitSight.ShowMesh();
                unit.RevealMyUnit();
                yield return new WaitForEndOfFrame();
            }
        }
    }

    void Update()
    {
        if (PlayActionPlan.sequencing || PlayActionPlan.ready)
        {
            return;
        }

        if (selection.menuOpen == false)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);

                SelectingUnit(out hit, ray);
            }
        }

        if (currentSelectedUnit)
        {
            if (Input.GetMouseButtonDown(1))
            {
                RaycastHit hit;
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);

                UnitOptions(out hit, ray);
            }
        }
    }

    void UnitOptions(out RaycastHit hit, Ray ray)
    {
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("Ground"))
            {
                selection.ShowOptions(hit.point);
                return;
            }
            else
            {
                Debug.Log($"Not hitting Ground tag {hit.collider.tag}");
            }
        }
        selection.HideOptions();
    }

    void SelectingUnit(out RaycastHit hit, Ray ray)
    {
        if (Physics.Raycast(ray, out hit))
        {
            Unit unit = hit.transform.GetComponent<Unit>();
            if (unit != null)
            {
                if (unit.idOwner == idOwner && unit.dead == false)
                {
                    if (currentSelectedUnit)
                        DeSelectUnit();

                    currentSelectedUnit = unit;
                    unit.Selected();
                }
                else
                {
                    DeSelectUnit();
                }
            }
            else
            {
                DeSelectUnit();
            }
        }
    }

    public void DeSelectUnit()
    {
        if (currentSelectedUnit)
            currentSelectedUnit.DeSelected();
        currentSelectedUnit = null;
        selection.HideOptions();
    }

    public void SetChoice(Vector3 targetPos, PlayerAction action, Vector3 virtualPosition)
    {
        client.SendServerRequest
            ("2 " + 
            currentSelectedUnit.unitID + " " +
            (int)action + " " +
            targetPos.x + " " +
            targetPos.y + " " +
            targetPos.z 
            );
        currentSelectedUnit.SetPlan(targetPos, action, virtualPosition);
    }

}
