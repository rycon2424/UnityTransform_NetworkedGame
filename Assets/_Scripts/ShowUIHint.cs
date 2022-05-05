using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ShowUIHint : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject hint;

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        hint.SetActive(true);
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        hint.SetActive(false);
    }

    public void OnDisable()
    {
        hint.SetActive(false);
    }
}
