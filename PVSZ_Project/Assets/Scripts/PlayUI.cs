using System.Collections.Generic;
using UnityEngine;

public class PlayUI : MonoBehaviour
{
    //-enabled on tech turn
    public EndTurn      endTurn;
    public GameObject   techCardsHolder;
    public RemoveCard   removeCard;
    
    //-enabled on alien turn
    public GameObject   powerCardsHolder;
    public FastForward  fastForward;
    
    public void OnAlienTurn()
    {
        endTurn.Dissable();
        //removeCard.Dissable();
        techCardsHolder.SetActive(false);
        
        powerCardsHolder.SetActive(true);
        fastForward.Enable();
    }
    
    public void OnTechTurn()
    {
        endTurn.Enable();
        //removeCard.Enable();
        techCardsHolder.SetActive(true);
        
        powerCardsHolder.SetActive(false);
        fastForward.Dissable();
    }
    
    public void OnFastForward()
    {
        powerCardsHolder.SetActive(false);
        fastForward.Dissable();
    }
}