using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    [SerializeField] float cameraSpeed = 2;
    [SerializeField] float wasdSpeed = 5;
    [Space]
    [SerializeField] float maxZoom = 10;
    [SerializeField] float minZoom = 20;

    private Vector3 newDistance;
    private Vector3 previousPos;

    void Update()
    {
        CameraMovement();
        Scroll();
    }

    void Scroll()
    {
        Vector2 scrollDelta = Input.mouseScrollDelta;
        if (scrollDelta.y != 0)
        {
            if (scrollDelta.y > 0.1f)
            {
                if (transform.position.y < minZoom)
                {
                    transform.position += new Vector3(0, scrollDelta.y, 0);
                }
            }
            else if (scrollDelta.y < 0.1f)
            {
                if (transform.position.y > maxZoom)
                {
                    transform.position += new Vector3(0, scrollDelta.y, 0);
                }
            }
        }
    }

    void CameraMovement()
    {
        transform.position += new Vector3(Input.GetAxis("Vertical"), 0, -Input.GetAxis("Horizontal")) * Time.deltaTime * wasdSpeed;
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
