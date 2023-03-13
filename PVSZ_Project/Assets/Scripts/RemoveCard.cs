using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RemoveCard : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData pointerData)
    {
        if (pointerData.button == PointerEventData.InputButton.Left)
        {
            GameManager.Instance.RemoveCardClicked();
        }
    }
    
    public void Enable()
    {
        gameObject.SetActive(true);
    }
    
    public void Dissable()
    {
        gameObject.SetActive(false);
    }
}