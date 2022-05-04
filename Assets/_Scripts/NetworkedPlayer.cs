using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class NetworkedPlayer : NetworkedObject
{
    [Space]
    [ReadOnly] public Camera cam;
    [ReadOnly] public Unit currentSelectedUnit;
    [ReadOnly] [SerializeField] UISelection selection;
    [Space]
    [SerializeField] GameObject unitMenu;

    void Start()
    {
        cam = Camera.main;
        selection = FindObjectOfType<UISelection>();

        // when online and use
        // StartCoroutine(EstablishConnection());
    }

    IEnumerator EstablishConnection()
    {
        // GET ID FROM SERVER (ID generated in server script)

        yield return new WaitForEndOfFrame();
        while (idOwner == 0) //if 0 = Still waiting for response
        {
            yield return new WaitForSeconds(0.5f);
        }
        List<Unit> myUnits = new List<Unit>();
        myUnits.AddRange(FindObjectsOfType<Unit>());
        foreach (Unit unit in myUnits)
        {
            if (unit.idOwner == idOwner)
            {
                unit.RevealMyUnit();
            }
        }
    }

    void Update()
    {
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

    public void SetChoice(Vector3 targetPos, PlayerAction action)
    {
        currentSelectedUnit.SetRoute(targetPos, action);
    }

}
