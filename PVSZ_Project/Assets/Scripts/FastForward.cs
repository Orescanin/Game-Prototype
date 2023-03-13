using UnityEngine;
using UnityEngine.EventSystems;

public class FastForward :  MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public PlayUI playUI;
    
    public void OnPointerDown(PointerEventData eventData)
    {
        // NOTE(sftl): nothing
    }
    
    public void OnPointerUp(PointerEventData pointerData)
    {
        if (pointerData.button == PointerEventData.InputButton.Left)
        {
            GameManager.Instance.FastForward();
            playUI.OnFastForward();
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