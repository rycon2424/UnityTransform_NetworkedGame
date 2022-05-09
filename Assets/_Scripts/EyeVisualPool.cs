using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeVisualPool : MonoBehaviour
{
    public List<GameObject> eyeVisuals = new List<GameObject>();

    private List<GameObject> eyeUsed = new List<GameObject>();

    public static EyeVisualPool instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
    }

    void Start()
    {
        foreach (var eye in eyeVisuals)
        {
            eye.SetActive(false);
        }
    }

    public void PlaceEye(Vector3 from, Vector3 to)
    {
        from += Vector3.up;
        to += Vector3.up;

        GameObject newEye = eyeVisuals[0];
        EyeVisual ev = newEye.GetComponent<EyeVisual>();

        newEye.SetActive(true);
        ev.SetEye(from, to);

        eyeVisuals.Remove(newEye);
        eyeUsed.Add(newEye);
    }

    public void ResetAllEyes()
    {
        foreach (GameObject eye in eyeUsed)
        {
            eye.SetActive(false);
            eyeVisuals.Add(eye);
        }
        eyeUsed.Clear();
    }

}
