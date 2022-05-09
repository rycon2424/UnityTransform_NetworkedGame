using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeVisual : MonoBehaviour
{
    [SerializeField] LineRenderer lineRenderer;

    public void SetEye(Vector3 from, Vector3 to)
    {
        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, from);
        lineRenderer.SetPosition(1, to);
        transform.position = to + Vector3.up;
    }

}