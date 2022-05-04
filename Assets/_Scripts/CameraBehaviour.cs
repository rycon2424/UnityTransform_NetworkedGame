using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    [SerializeField] float cameraSpeed = 2;

    private Vector3 newDistance;
    private Vector3 previousPos;

    void Update()
    {
        CameraMovement();
    }

    void CameraMovement()
    {
        if (Input.GetMouseButton(2))
        {
            newDistance = Input.mousePosition;
            float distance = Vector3.Distance(previousPos, newDistance);
            if (Input.GetMouseButtonDown(2))
            {
                newDistance = Vector3.zero;
                previousPos = Vector3.zero;
            }
            if (distance > 0.1f)
            {
                Vector3 temp = newDistance - previousPos;
                transform.position += new Vector3(-temp.y, 0, temp.x) * Time.deltaTime * cameraSpeed;
            }
            previousPos = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(0))
        {
            newDistance = Vector3.zero;
            previousPos = Vector3.zero;
        }
    }
}
