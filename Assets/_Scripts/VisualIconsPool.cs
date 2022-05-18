using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
class VisualPool
{
    public int id;
    public List<GameObject> visuals = new List<GameObject>();
    public List<GameObject> visualsUsed = new List<GameObject>();

    public void HideVisuals()
    {
        foreach (GameObject v in visuals)
        {
            v.SetActive(false);
        }
    }

    public void ResetPool()
    {
        foreach (GameObject v in visualsUsed)
        {
            v.SetActive(false);
            visuals.Add(v);
        }
        visualsUsed.Clear();
    }
}


public class VisualIconsPool : MonoBehaviour
{
    [SerializeField] private List<VisualPool> visualPool = new List<VisualPool>();

    public static VisualIconsPool instance;

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
        foreach (var pool in visualPool)
        {
            pool.HideVisuals();
        }
    }

    public void PlaceVisual(Vector3 from, Vector3 to, int id)
    {
        from += Vector3.up;
        to += Vector3.up;

        foreach (VisualPool pool in visualPool)
        {
            if (pool.id == id)
            {
                GameObject visual = pool.visuals[0];
                VisualIcon visualicon = visual.GetComponent<VisualIcon>();
                visual.SetActive(true);
                visualicon.SetVisual(from, to);
                pool.visuals.Remove(visual);
                pool.visualsUsed.Add(visual);
                return;
            }
        }
    }

    public void ResetAllPools()
    {
        foreach (var pool in visualPool)
        {
            pool.ResetPool();
        }
    }

}
