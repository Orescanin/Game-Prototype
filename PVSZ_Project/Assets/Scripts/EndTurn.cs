using UnityEngine;
using UnityEngine.EventSystems;

public class EndTurn : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    
    public void OnPointerDown(PointerEventData eventData)
    {
        // NOTE(sftl): nothing
    }
    
    public void OnPointerUp(PointerEventData pointerData)
    {
        if (pointerData.button == PointerEventData.InputButton.Left)
        {
            GameManager.Instance.EndTechTurn();
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